using HotelLakeview.Application.Contracts.Images;

namespace HotelLakeview.Application.Abstractions;

public interface IRoomImageService
{
    Task<IReadOnlyList<RoomImageDto>> GetByRoomIdAsync(Guid roomId, CancellationToken cancellationToken);

    Task<RoomImageDto> UploadAsync(Guid roomId, string fileName, string contentType, long sizeBytes, Stream content, CancellationToken cancellationToken);

    Task<(Stream Content, string ContentType, string FileName)> OpenImageAsync(Guid roomId, Guid imageId, CancellationToken cancellationToken);

    Task DeleteAsync(Guid roomId, Guid imageId, CancellationToken cancellationToken);
}
