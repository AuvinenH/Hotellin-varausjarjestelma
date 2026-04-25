using HotelLakeview.Domain.Entities;
using HotelLakeview.Domain.Enums;
using HotelLakeview.Domain.ValueObjects;

namespace HotelLakeview.Application.Abstractions;

public interface IRoomRepository
{
    Task<IReadOnlyList<Room>> GetAllAsync(CancellationToken cancellationToken);

    Task<Room?> GetByIdAsync(Guid id, CancellationToken cancellationToken);

    Task<Room?> GetByNumberAsync(string normalizedRoomNumber, CancellationToken cancellationToken);

    Task<IReadOnlyList<Room>> GetAvailableAsync(DateRange dateRange, int guestCount, RoomCategory? category, CancellationToken cancellationToken);

    Task<int> CountAsync(CancellationToken cancellationToken);

    Task AddAsync(Room room, CancellationToken cancellationToken);

    void Remove(Room room);

    Task SaveChangesAsync(CancellationToken cancellationToken);
}
