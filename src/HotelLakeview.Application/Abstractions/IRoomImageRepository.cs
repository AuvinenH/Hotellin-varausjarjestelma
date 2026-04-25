using HotelLakeview.Domain.Entities;

namespace HotelLakeview.Application.Abstractions;

public interface IRoomImageRepository
{
    Task<IReadOnlyList<RoomImage>> GetByRoomIdAsync(Guid roomId, CancellationToken cancellationToken);

    Task<RoomImage?> GetByIdAsync(Guid imageId, CancellationToken cancellationToken);

    Task AddAsync(RoomImage image, CancellationToken cancellationToken);

    void Remove(RoomImage image);

    Task SaveChangesAsync(CancellationToken cancellationToken);
}
