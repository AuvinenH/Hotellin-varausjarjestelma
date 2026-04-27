using HotelLakeview.Application.Abstractions;
using HotelLakeview.Application.Common;
using HotelLakeview.Application.Contracts.Images;
using MediatR;

namespace HotelLakeview.Application.CQRS.RoomImages;

public sealed record GetRoomImagesQuery(Guid RoomId) : IRequest<Result<IReadOnlyList<RoomImageDto>>>;

public sealed class GetRoomImagesQueryHandler : IRequestHandler<GetRoomImagesQuery, Result<IReadOnlyList<RoomImageDto>>>
{
    private readonly IRoomImageService _roomImageService;

    public GetRoomImagesQueryHandler(IRoomImageService roomImageService)
    {
        _roomImageService = roomImageService;
    }

    public Task<Result<IReadOnlyList<RoomImageDto>>> Handle(GetRoomImagesQuery request, CancellationToken cancellationToken)
        => _roomImageService.GetByRoomIdAsync(request.RoomId, cancellationToken);
}

public sealed record UploadRoomImageCommand(
    Guid RoomId,
    string FileName,
    string ContentType,
    long SizeBytes,
    Stream Content) : IRequest<Result<RoomImageDto>>;

public sealed class UploadRoomImageCommandHandler : IRequestHandler<UploadRoomImageCommand, Result<RoomImageDto>>
{
    private readonly IRoomImageService _roomImageService;

    public UploadRoomImageCommandHandler(IRoomImageService roomImageService)
    {
        _roomImageService = roomImageService;
    }

    public Task<Result<RoomImageDto>> Handle(UploadRoomImageCommand request, CancellationToken cancellationToken)
        => _roomImageService.UploadAsync(
            request.RoomId,
            request.FileName,
            request.ContentType,
            request.SizeBytes,
            request.Content,
            cancellationToken);
}

public sealed record OpenRoomImageQuery(Guid RoomId, Guid ImageId) : IRequest<Result<(Stream Content, string ContentType, string FileName)>>;

public sealed class OpenRoomImageQueryHandler : IRequestHandler<OpenRoomImageQuery, Result<(Stream Content, string ContentType, string FileName)>>
{
    private readonly IRoomImageService _roomImageService;

    public OpenRoomImageQueryHandler(IRoomImageService roomImageService)
    {
        _roomImageService = roomImageService;
    }

    public Task<Result<(Stream Content, string ContentType, string FileName)>> Handle(OpenRoomImageQuery request, CancellationToken cancellationToken)
        => _roomImageService.OpenImageAsync(request.RoomId, request.ImageId, cancellationToken);
}

public sealed record DeleteRoomImageCommand(Guid RoomId, Guid ImageId) : IRequest<Result>;

public sealed class DeleteRoomImageCommandHandler : IRequestHandler<DeleteRoomImageCommand, Result>
{
    private readonly IRoomImageService _roomImageService;

    public DeleteRoomImageCommandHandler(IRoomImageService roomImageService)
    {
        _roomImageService = roomImageService;
    }

    public Task<Result> Handle(DeleteRoomImageCommand request, CancellationToken cancellationToken)
        => _roomImageService.DeleteAsync(request.RoomId, request.ImageId, cancellationToken);
}
