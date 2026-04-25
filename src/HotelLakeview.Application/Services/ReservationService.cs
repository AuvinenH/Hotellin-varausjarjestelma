using HotelLakeview.Application.Abstractions;
using HotelLakeview.Application.Contracts.Reservations;
using HotelLakeview.Application.Exceptions;
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

    public async Task<IReadOnlyList<ReservationDto>> GetAllAsync(CancellationToken cancellationToken)
    {
        var reservations = await _reservationRepository.GetAllAsync(cancellationToken);
        return reservations.Select(Map).ToList();
    }

    public async Task<ReservationDto> GetByIdAsync(Guid id, CancellationToken cancellationToken)
    {
        var reservation = await _reservationRepository.GetByIdAsync(id, cancellationToken)
            ?? throw new NotFoundException($"Reservation '{id}' was not found.");

        return Map(reservation);
    }

    public async Task<ReservationDto> CreateAsync(CreateReservationRequest request, CancellationToken cancellationToken)
    {
        var room = await _roomRepository.GetByIdAsync(request.RoomId, cancellationToken)
            ?? throw new NotFoundException($"Room '{request.RoomId}' was not found.");

        _ = await _customerRepository.GetByIdAsync(request.CustomerId, cancellationToken)
            ?? throw new NotFoundException($"Customer '{request.CustomerId}' was not found.");

        EnsureGuestCountFitsRoom(request.GuestCount, room.MaxGuests);

        var dateRange = new DateRange(request.CheckInDate, request.CheckOutDate);

        if (await _reservationRepository.HasOverlapAsync(room.Id, dateRange, null, cancellationToken))
        {
            throw new ConflictException("Selected room is already booked for the requested dates.");
        }

        var totalPrice = _pricingService.CalculateTotal(room.BasePricePerNight, dateRange);
        var reservation = new Reservation(Guid.NewGuid(), room.Id, request.CustomerId, dateRange, request.GuestCount, totalPrice);

        var nights = dateRange
            .EnumerateNights()
            .Select(date => new ReservationNight(reservation.Id, reservation.RoomId, date))
            .ToArray();

        await _reservationRepository.AddAsync(reservation, nights, cancellationToken);

        return Map(reservation);
    }

    public async Task<ReservationDto> UpdateAsync(Guid id, UpdateReservationRequest request, CancellationToken cancellationToken)
    {
        var reservation = await _reservationRepository.GetByIdAsync(id, cancellationToken)
            ?? throw new NotFoundException($"Reservation '{id}' was not found.");

        if (reservation.Status == ReservationStatus.Cancelled)
        {
            throw new ConflictException("Cancelled reservation cannot be modified.");
        }

        var room = await _roomRepository.GetByIdAsync(request.RoomId, cancellationToken)
            ?? throw new NotFoundException($"Room '{request.RoomId}' was not found.");

        EnsureGuestCountFitsRoom(request.GuestCount, room.MaxGuests);

        var dateRange = new DateRange(request.CheckInDate, request.CheckOutDate);

        if (await _reservationRepository.HasOverlapAsync(room.Id, dateRange, reservation.Id, cancellationToken))
        {
            throw new ConflictException("Selected room is already booked for the updated dates.");
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
            throw new ConflictException("Reservation could not be updated due to a booking conflict.");
        }

        return Map(reservation);
    }

    public async Task CancelAsync(Guid id, CancellationToken cancellationToken)
    {
        var reservation = await _reservationRepository.GetByIdAsync(id, cancellationToken)
            ?? throw new NotFoundException($"Reservation '{id}' was not found.");

        reservation.Cancel();
        await _reservationRepository.SaveChangesAsync(cancellationToken);
    }

    private static void EnsureGuestCountFitsRoom(int guestCount, int roomMaxGuests)
    {
        if (guestCount > roomMaxGuests)
        {
            throw new ArgumentException($"Guest count exceeds room capacity ({roomMaxGuests}).");
        }
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
