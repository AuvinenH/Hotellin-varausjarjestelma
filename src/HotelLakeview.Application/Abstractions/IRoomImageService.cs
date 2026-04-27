using HotelLakeview.Application.Contracts.Images;
using HotelLakeview.Application.Common;

namespace HotelLakeview.Application.Abstractions;

public interface IRoomImageService
{
    Task<Result<IReadOnlyList<RoomImageDto>>> GetByRoomIdAsync(Guid roomId, CancellationToken cancellationToken);

    Task<Result<RoomImageDto>> UploadAsync(Guid roomId, string fileName, string contentType, long sizeBytes, Stream content, CancellationToken cancellationToken);

    Task<Result<(Stream Content, string ContentType, string FileName)>> OpenImageAsync(Guid roomId, Guid imageId, CancellationToken cancellationToken);

    Task<Result> DeleteAsync(Guid roomId, Guid imageId, CancellationToken cancellationToken);
}
