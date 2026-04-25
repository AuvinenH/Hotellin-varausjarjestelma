using HotelLakeview.Application.Abstractions;
using HotelLakeview.Domain.Entities;
using HotelLakeview.Infrastructure.Persistence;
using Microsoft.EntityFrameworkCore;

namespace HotelLakeview.Infrastructure.Repositories;

public class RoomImageRepository : IRoomImageRepository
{
    private readonly HotelDbContext _dbContext;

    public RoomImageRepository(HotelDbContext dbContext)
    {
        _dbContext = dbContext;
    }

    public async Task<IReadOnlyList<RoomImage>> GetByRoomIdAsync(Guid roomId, CancellationToken cancellationToken)
    {
        return await _dbContext.RoomImages
            .AsNoTracking()
            .Where(image => image.RoomId == roomId)
            .OrderByDescending(image => image.UploadedAtUtc)
            .ToListAsync(cancellationToken);
    }

    public Task<RoomImage?> GetByIdAsync(Guid imageId, CancellationToken cancellationToken)
    {
        return _dbContext.RoomImages.FirstOrDefaultAsync(image => image.Id == imageId, cancellationToken);
    }

    public Task AddAsync(RoomImage image, CancellationToken cancellationToken)
    {
        return _dbContext.RoomImages.AddAsync(image, cancellationToken).AsTask();
    }

    public void Remove(RoomImage image)
    {
        _dbContext.RoomImages.Remove(image);
    }

    public Task SaveChangesAsync(CancellationToken cancellationToken)
    {
        return _dbContext.SaveChangesAsync(cancellationToken);
    }
}
