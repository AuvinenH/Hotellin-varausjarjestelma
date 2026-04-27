using HotelLakeview.Application.Abstractions;
using HotelLakeview.Application.Common;
using HotelLakeview.Application.Contracts.Reservations;
using HotelLakeview.Domain.Entities;
using HotelLakeview.Domain.Enums;
using HotelLakeview.Domain.ValueObjects;

namespace HotelLakeview.Application.Services;

public class ReservationService : IReservationService
{
    private readonly ICustomerRepository _customerRepository;
    private readonly IPricingService _pricingService;
    private readonly IReservationRepository _reservationRepository;
    private readonly IRoomRepository _roomRepository;

    public ReservationService(
        IReservationRepository reservationRepository,
        IRoomRepository roomRepository,
        ICustomerRepository customerRepository,
        IPricingService pricingService)
    {
        _reservationRepository = reservationRepository;
        _roomRepository = roomRepository;
        _customerRepository = customerRepository;
        _pricingService = pricingService;
    }

    public async Task<Result<IReadOnlyList<ReservationDto>>> GetAllAsync(CancellationToken cancellationToken)
    {
        var reservations = await _reservationRepository.GetAllAsync(cancellationToken);
        return Result<IReadOnlyList<ReservationDto>>.Success(reservations.Select(Map).ToList());
    }

    public async Task<Result<ReservationDto>> GetByIdAsync(Guid id, CancellationToken cancellationToken)
    {
        var reservation = await _reservationRepository.GetByIdAsync(id, cancellationToken);

        if (reservation is null)
        {
            return Result<ReservationDto>.Failure(ResultError.NotFound("reservation.not_found", $"Reservation '{id}' was not found."));
        }

        return Result<ReservationDto>.Success(Map(reservation));
    }

    public async Task<Result<ReservationDto>> CreateAsync(CreateReservationRequest request, CancellationToken cancellationToken)
    {
        var room = await _roomRepository.GetByIdAsync(request.RoomId, cancellationToken);
        if (room is null)
        {
            return Result<ReservationDto>.Failure(ResultError.NotFound("reservation.room_not_found", $"Room '{request.RoomId}' was not found."));
        }

        var customer = await _customerRepository.GetByIdAsync(request.CustomerId, cancellationToken);
        if (customer is null)
        {
            return Result<ReservationDto>.Failure(ResultError.NotFound("reservation.customer_not_found", $"Customer '{request.CustomerId}' was not found."));
        }

        var guestCountError = ValidateGuestCountFitsRoom(request.GuestCount, room.MaxGuests);
        if (guestCountError is not null)
        {
            return Result<ReservationDto>.Failure(guestCountError);
        }

        DateRange dateRange;
        try
        {
            dateRange = new DateRange(request.CheckInDate, request.CheckOutDate);
        }
        catch (ArgumentException exception)
        {
            return Result<ReservationDto>.Failure(ResultError.Validation("reservation.invalid_date_range", exception.Message));
        }

        if (await _reservationRepository.HasOverlapAsync(room.Id, dateRange, null, cancellationToken))
        {
            return Result<ReservationDto>.Failure(ResultError.Conflict("reservation.overlap", "Selected room is already booked for the requested dates."));
        }

        var totalPrice = _pricingService.CalculateTotal(room.BasePricePerNight, dateRange);
        var reservation = new Reservation(Guid.NewGuid(), room.Id, request.CustomerId, dateRange, request.GuestCount, totalPrice);

        var nights = dateRange
            .EnumerateNights()
            .Select(date => new ReservationNight(reservation.Id, reservation.RoomId, date))
            .ToArray();

        await _reservationRepository.AddAsync(reservation, nights, cancellationToken);

        return Result<ReservationDto>.Success(Map(reservation));
    }

    public async Task<Result<ReservationDto>> UpdateAsync(Guid id, UpdateReservationRequest request, CancellationToken cancellationToken)
    {
        var reservation = await _reservationRepository.GetByIdAsync(id, cancellationToken);
        if (reservation is null)
        {
            return Result<ReservationDto>.Failure(ResultError.NotFound("reservation.not_found", $"Reservation '{id}' was not found."));
        }

        if (reservation.Status == ReservationStatus.Cancelled)
        {
            return Result<ReservationDto>.Failure(ResultError.Conflict("reservation.cancelled", "Cancelled reservation cannot be modified."));
        }

        var room = await _roomRepository.GetByIdAsync(request.RoomId, cancellationToken);
        if (room is null)
        {
            return Result<ReservationDto>.Failure(ResultError.NotFound("reservation.room_not_found", $"Room '{request.RoomId}' was not found."));
        }

        var guestCountError = ValidateGuestCountFitsRoom(request.GuestCount, room.MaxGuests);
        if (guestCountError is not null)
        {
            return Result<ReservationDto>.Failure(guestCountError);
        }

        DateRange dateRange;
        try
        {
            dateRange = new DateRange(request.CheckInDate, request.CheckOutDate);
        }
        catch (ArgumentException exception)
        {
            return Result<ReservationDto>.Failure(ResultError.Validation("reservation.invalid_date_range", exception.Message));
        }

        if (await _reservationRepository.HasOverlapAsync(room.Id, dateRange, reservation.Id, cancellationToken))
        {
            return Result<ReservationDto>.Failure(ResultError.Conflict("reservation.overlap", "Selected room is already booked for the updated dates."));
        }

        var totalPrice = _pricingService.CalculateTotal(room.BasePricePerNight, dateRange);

        reservation.Update(room.Id, dateRange, request.GuestCount, totalPrice);

        var nights = dateRange
            .EnumerateNights()
            .Select(date => new ReservationNight(reservation.Id, reservation.RoomId, date))
            .ToArray();

        var succeeded = await _reservationRepository.TryUpdateAsync(reservation, nights, cancellationToken);

        if (!succeeded)
        {
            return Result<ReservationDto>.Failure(ResultError.Conflict("reservation.update_conflict", "Reservation could not be updated due to a booking conflict."));
        }

        return Result<ReservationDto>.Success(Map(reservation));
    }

    public async Task<Result> CancelAsync(Guid id, CancellationToken cancellationToken)
    {
        var reservation = await _reservationRepository.GetByIdAsync(id, cancellationToken);
        if (reservation is null)
        {
            return Result.Failure(ResultError.NotFound("reservation.not_found", $"Reservation '{id}' was not found."));
        }

        reservation.Cancel();
        await _reservationRepository.SaveChangesAsync(cancellationToken);

        return Result.Success();
    }

    private static ResultError? ValidateGuestCountFitsRoom(int guestCount, int roomMaxGuests)
    {
        if (guestCount > roomMaxGuests)
        {
            return ResultError.Validation("reservation.guest_count_exceeds_capacity", $"Guest count exceeds room capacity ({roomMaxGuests}).");
        }

        return null;
    }

    private static ReservationDto Map(Reservation reservation)
    {
        return new ReservationDto(
            reservation.Id,
            reservation.RoomId,
            reservation.CustomerId,
            reservation.CheckInDate,
            reservation.CheckOutDate,
            reservation.GuestCount,
            reservation.TotalPrice,
            reservation.Status,
            reservation.CreatedAtUtc);
    }
}
