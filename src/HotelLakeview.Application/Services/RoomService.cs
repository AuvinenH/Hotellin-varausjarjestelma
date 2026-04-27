using HotelLakeview.Application.Abstractions;
using HotelLakeview.Application.Common;
using HotelLakeview.Application.Contracts.Rooms;
using HotelLakeview.Domain.Entities;
using HotelLakeview.Domain.Enums;
using HotelLakeview.Domain.ValueObjects;

namespace HotelLakeview.Application.Services;

public class RoomService : IRoomService
{
    private readonly IRoomRepository _roomRepository;

    public RoomService(IRoomRepository roomRepository)
    {
        _roomRepository = roomRepository;
    }

    public async Task<Result<IReadOnlyList<RoomDto>>> GetAllAsync(CancellationToken cancellationToken)
    {
        var rooms = await _roomRepository.GetAllAsync(cancellationToken);
        return Result<IReadOnlyList<RoomDto>>.Success(rooms.Select(Map).ToList());
    }

    public async Task<Result<RoomDto>> GetByIdAsync(Guid id, CancellationToken cancellationToken)
    {
        var room = await _roomRepository.GetByIdAsync(id, cancellationToken);

        if (room is null)
        {
            return Result<RoomDto>.Failure(ResultError.NotFound("room.not_found", $"Room '{id}' was not found."));
        }

        return Result<RoomDto>.Success(Map(room));
    }

    public async Task<Result<RoomDto>> CreateAsync(CreateRoomRequest request, CancellationToken cancellationToken)
    {
        var normalizedNumber = request.Number.Trim().ToUpperInvariant();
        var existing = await _roomRepository.GetByNumberAsync(normalizedNumber, cancellationToken);

        if (existing is not null)
        {
            return Result<RoomDto>.Failure(ResultError.Conflict("room.number_conflict", "Room with the same number already exists."));
        }

        Room room;
        try
        {
            room = new Room(
                Guid.NewGuid(),
                request.Number,
                request.Category,
                request.MaxGuests,
                request.BasePricePerNight,
                request.Description);
        }
        catch (ArgumentException exception)
        {
            return Result<RoomDto>.Failure(ResultError.Validation("room.invalid", exception.Message));
        }

        await _roomRepository.AddAsync(room, cancellationToken);
        await _roomRepository.SaveChangesAsync(cancellationToken);

        return Result<RoomDto>.Success(Map(room));
    }

    public async Task<Result<RoomDto>> UpdateAsync(Guid id, UpdateRoomRequest request, CancellationToken cancellationToken)
    {
        var room = await _roomRepository.GetByIdAsync(id, cancellationToken);

        if (room is null)
        {
            return Result<RoomDto>.Failure(ResultError.NotFound("room.not_found", $"Room '{id}' was not found."));
        }

        var normalizedNumber = request.Number.Trim().ToUpperInvariant();
        var existing = await _roomRepository.GetByNumberAsync(normalizedNumber, cancellationToken);

        if (existing is not null && existing.Id != id)
        {
            return Result<RoomDto>.Failure(ResultError.Conflict("room.number_conflict", "Room with the same number already exists."));
        }

        try
        {
            room.UpdateDetails(
                request.Number,
                request.Category,
                request.MaxGuests,
                request.BasePricePerNight,
                request.Description);
        }
        catch (ArgumentException exception)
        {
            return Result<RoomDto>.Failure(ResultError.Validation("room.invalid", exception.Message));
        }

        await _roomRepository.SaveChangesAsync(cancellationToken);

        return Result<RoomDto>.Success(Map(room));
    }

    public async Task<Result> DeleteAsync(Guid id, CancellationToken cancellationToken)
    {
        var room = await _roomRepository.GetByIdAsync(id, cancellationToken);

        if (room is null)
        {
            return Result.Failure(ResultError.NotFound("room.not_found", $"Room '{id}' was not found."));
        }

        _roomRepository.Remove(room);
        await _roomRepository.SaveChangesAsync(cancellationToken);

        return Result.Success();
    }

    public async Task<Result<IReadOnlyList<RoomDto>>> GetAvailableAsync(DateOnly checkInDate, DateOnly checkOutDate, int guestCount, RoomCategory? category, CancellationToken cancellationToken)
    {
        DateRange dateRange;
        try
        {
            dateRange = new DateRange(checkInDate, checkOutDate);
        }
        catch (ArgumentException exception)
        {
            return Result<IReadOnlyList<RoomDto>>.Failure(ResultError.Validation("room.availability.invalid_date_range", exception.Message));
        }

        var rooms = await _roomRepository.GetAvailableAsync(dateRange, guestCount, category, cancellationToken);
        return Result<IReadOnlyList<RoomDto>>.Success(rooms.Select(Map).ToList());
    }

    private static RoomDto Map(Room room)
    {
        return new RoomDto(
            room.Id,
            room.Number,
            room.Category,
            room.MaxGuests,
            room.BasePricePerNight,
            room.Description);
    }
}
