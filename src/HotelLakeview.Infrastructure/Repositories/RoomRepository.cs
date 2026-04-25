using HotelLakeview.Application.Abstractions;
using HotelLakeview.Domain.Entities;
using HotelLakeview.Domain.Enums;
using HotelLakeview.Domain.ValueObjects;
using HotelLakeview.Infrastructure.Persistence;
using Microsoft.EntityFrameworkCore;

namespace HotelLakeview.Infrastructure.Repositories;

public class RoomRepository : IRoomRepository
{
    private readonly HotelDbContext _dbContext;

    public RoomRepository(HotelDbContext dbContext)
    {
        _dbContext = dbContext;
    }

    public async Task<IReadOnlyList<Room>> GetAllAsync(CancellationToken cancellationToken)
    {
        return await _dbContext.Rooms
            .AsNoTracking()
            .OrderBy(room => room.Number)
            .ToListAsync(cancellationToken);
    }

    public Task<Room?> GetByIdAsync(Guid id, CancellationToken cancellationToken)
    {
        return _dbContext.Rooms.FirstOrDefaultAsync(room => room.Id == id, cancellationToken);
    }

    public Task<Room?> GetByNumberAsync(string normalizedRoomNumber, CancellationToken cancellationToken)
    {
        return _dbContext.Rooms.FirstOrDefaultAsync(
            room => room.Number == normalizedRoomNumber,
            cancellationToken);
    }

    public async Task<IReadOnlyList<Room>> GetAvailableAsync(DateRange dateRange, int guestCount, RoomCategory? category, CancellationToken cancellationToken)
    {
        var query = _dbContext.Rooms
            .AsNoTracking()
            .Where(room => room.MaxGuests >= guestCount);

        if (category.HasValue)
        {
            query = query.Where(room => room.Category == category.Value);
        }

        query = query.Where(room => !_dbContext.ReservationNights.Any(night =>
            night.RoomId == room.Id
            && night.NightDate >= dateRange.CheckInDate
            && night.NightDate < dateRange.CheckOutDate
            && night.Reservation!.Status != ReservationStatus.Cancelled));

        return await query
            .OrderBy(room => room.Number)
            .ToListAsync(cancellationToken);
    }

    public Task<int> CountAsync(CancellationToken cancellationToken)
    {
        return _dbContext.Rooms.CountAsync(cancellationToken);
    }

    public Task AddAsync(Room room, CancellationToken cancellationToken)
    {
        return _dbContext.Rooms.AddAsync(room, cancellationToken).AsTask();
    }

    public void Remove(Room room)
    {
        _dbContext.Rooms.Remove(room);
    }

    public Task SaveChangesAsync(CancellationToken cancellationToken)
    {
        return _dbContext.SaveChangesAsync(cancellationToken);
    }
}
