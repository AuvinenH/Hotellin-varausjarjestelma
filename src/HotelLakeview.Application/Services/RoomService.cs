using HotelLakeview.Application.Abstractions;
using HotelLakeview.Application.Contracts.Rooms;
using HotelLakeview.Application.Exceptions;
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

    public async Task<IReadOnlyList<RoomDto>> GetAllAsync(CancellationToken cancellationToken)
    {
        var rooms = await _roomRepository.GetAllAsync(cancellationToken);
        return rooms.Select(Map).ToList();
    }

    public async Task<RoomDto> GetByIdAsync(Guid id, CancellationToken cancellationToken)
    {
        var room = await _roomRepository.GetByIdAsync(id, cancellationToken)
            ?? throw new NotFoundException($"Room '{id}' was not found.");

        return Map(room);
    }

    public async Task<RoomDto> CreateAsync(CreateRoomRequest request, CancellationToken cancellationToken)
    {
        var normalizedNumber = request.Number.Trim().ToUpperInvariant();
        var existing = await _roomRepository.GetByNumberAsync(normalizedNumber, cancellationToken);

        if (existing is not null)
        {
            throw new ConflictException("Room with the same number already exists.");
        }

        var room = new Room(
            Guid.NewGuid(),
            request.Number,
            request.Category,
            request.MaxGuests,
            request.BasePricePerNight,
            request.Description);

        await _roomRepository.AddAsync(room, cancellationToken);
        await _roomRepository.SaveChangesAsync(cancellationToken);

        return Map(room);
    }

    public async Task<RoomDto> UpdateAsync(Guid id, UpdateRoomRequest request, CancellationToken cancellationToken)
    {
        var room = await _roomRepository.GetByIdAsync(id, cancellationToken)
            ?? throw new NotFoundException($"Room '{id}' was not found.");

        var normalizedNumber = request.Number.Trim().ToUpperInvariant();
        var existing = await _roomRepository.GetByNumberAsync(normalizedNumber, cancellationToken);

        if (existing is not null && existing.Id != id)
        {
            throw new ConflictException("Room with the same number already exists.");
        }

        room.UpdateDetails(
            request.Number,
            request.Category,
            request.MaxGuests,
            request.BasePricePerNight,
            request.Description);

        await _roomRepository.SaveChangesAsync(cancellationToken);

        return Map(room);
    }

    public async Task DeleteAsync(Guid id, CancellationToken cancellationToken)
    {
        var room = await _roomRepository.GetByIdAsync(id, cancellationToken)
            ?? throw new NotFoundException($"Room '{id}' was not found.");

        _roomRepository.Remove(room);
        await _roomRepository.SaveChangesAsync(cancellationToken);
    }

    public async Task<IReadOnlyList<RoomDto>> GetAvailableAsync(DateOnly checkInDate, DateOnly checkOutDate, int guestCount, RoomCategory? category, CancellationToken cancellationToken)
    {
        var dateRange = new DateRange(checkInDate, checkOutDate);
        var rooms = await _roomRepository.GetAvailableAsync(dateRange, guestCount, category, cancellationToken);
        return rooms.Select(Map).ToList();
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
