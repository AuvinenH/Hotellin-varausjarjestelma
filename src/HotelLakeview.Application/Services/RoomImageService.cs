using HotelLakeview.Application.Abstractions;
using HotelLakeview.Application.Common;
using HotelLakeview.Application.Contracts.Images;
using HotelLakeview.Domain.Entities;

namespace HotelLakeview.Application.Services;

public class RoomImageService : IRoomImageService
{
    private const long MaxFileSizeBytes = 5 * 1024 * 1024;

    private static readonly HashSet<string> AllowedContentTypes = new(StringComparer.OrdinalIgnoreCase)
    {
        "image/jpeg",
        "image/png",
        "image/webp",
        "image/gif",
    };

    private readonly IRoomImageRepository _imageRepository;
    private readonly IRoomRepository _roomRepository;
    private readonly IRoomImageStorage _storage;

    public RoomImageService(IRoomRepository roomRepository, IRoomImageRepository imageRepository, IRoomImageStorage storage)
    {
        _roomRepository = roomRepository;
        _imageRepository = imageRepository;
        _storage = storage;
    }

    public async Task<Result<IReadOnlyList<RoomImageDto>>> GetByRoomIdAsync(Guid roomId, CancellationToken cancellationToken)
    {
        var roomExistsResult = await EnsureRoomExists(roomId, cancellationToken);
        if (roomExistsResult.IsFailure)
        {
            return Result<IReadOnlyList<RoomImageDto>>.Failure(roomExistsResult.Error!);
        }

        var images = await _imageRepository.GetByRoomIdAsync(roomId, cancellationToken);
        return Result<IReadOnlyList<RoomImageDto>>.Success(images.Select(Map).ToList());
    }

    public async Task<Result<RoomImageDto>> UploadAsync(Guid roomId, string fileName, string contentType, long sizeBytes, Stream content, CancellationToken cancellationToken)
    {
        var roomExistsResult = await EnsureRoomExists(roomId, cancellationToken);
        if (roomExistsResult.IsFailure)
        {
            return Result<RoomImageDto>.Failure(roomExistsResult.Error!);
        }

        if (string.IsNullOrWhiteSpace(fileName))
        {
            return Result<RoomImageDto>.Failure(ResultError.Validation("room_image.file_name_required", "File name is required."));
        }

        if (!AllowedContentTypes.Contains(contentType))
        {
            return Result<RoomImageDto>.Failure(ResultError.Validation("room_image.unsupported_type", "Unsupported file type."));
        }

        if (sizeBytes <= 0 || sizeBytes > MaxFileSizeBytes)
        {
            return Result<RoomImageDto>.Failure(ResultError.Validation("room_image.invalid_size", "Image size must be between 1 byte and 5 MB."));
        }

        var extension = Path.GetExtension(fileName);
        var storedFileName = string.IsNullOrWhiteSpace(extension)
            ? $"{roomId}/{Guid.NewGuid():N}"
            : $"{roomId}/{Guid.NewGuid():N}{extension.ToLowerInvariant()}";

        await _storage.SaveAsync(storedFileName, content, cancellationToken);

        var image = new RoomImage(
            Guid.NewGuid(),
            roomId,
            fileName,
            storedFileName,
            contentType,
            sizeBytes);

        await _imageRepository.AddAsync(image, cancellationToken);
        await _imageRepository.SaveChangesAsync(cancellationToken);

        return Result<RoomImageDto>.Success(Map(image));
    }

    public async Task<Result<(Stream Content, string ContentType, string FileName)>> OpenImageAsync(Guid roomId, Guid imageId, CancellationToken cancellationToken)
    {
        var roomExistsResult = await EnsureRoomExists(roomId, cancellationToken);
        if (roomExistsResult.IsFailure)
        {
            return Result<(Stream Content, string ContentType, string FileName)>.Failure(roomExistsResult.Error!);
        }

        var image = await _imageRepository.GetByIdAsync(imageId, cancellationToken);
        if (image is null)
        {
            return Result<(Stream Content, string ContentType, string FileName)>.Failure(ResultError.NotFound("room_image.not_found", $"Image '{imageId}' was not found."));
        }

        if (image.RoomId != roomId)
        {
            return Result<(Stream Content, string ContentType, string FileName)>.Failure(ResultError.NotFound("room_image.not_found_for_room", $"Image '{imageId}' was not found for room '{roomId}'."));
        }

        var stream = await _storage.OpenReadAsync(image.StoredFileName, cancellationToken);
        return Result<(Stream Content, string ContentType, string FileName)>.Success((stream, image.ContentType, image.FileName));
    }

    public async Task<Result> DeleteAsync(Guid roomId, Guid imageId, CancellationToken cancellationToken)
    {
        var roomExistsResult = await EnsureRoomExists(roomId, cancellationToken);
        if (roomExistsResult.IsFailure)
        {
            return roomExistsResult;
        }

        var image = await _imageRepository.GetByIdAsync(imageId, cancellationToken);
        if (image is null)
        {
            return Result.Failure(ResultError.NotFound("room_image.not_found", $"Image '{imageId}' was not found."));
        }

        if (image.RoomId != roomId)
        {
            return Result.Failure(ResultError.NotFound("room_image.not_found_for_room", $"Image '{imageId}' was not found for room '{roomId}'."));
        }

        await _storage.DeleteAsync(image.StoredFileName, cancellationToken);
        _imageRepository.Remove(image);
        await _imageRepository.SaveChangesAsync(cancellationToken);

        return Result.Success();
    }

    private async Task<Result> EnsureRoomExists(Guid roomId, CancellationToken cancellationToken)
    {
        var room = await _roomRepository.GetByIdAsync(roomId, cancellationToken);

        if (room is null)
        {
            return Result.Failure(ResultError.NotFound("room.not_found", $"Room '{roomId}' was not found."));
        }

        return Result.Success();
    }

    private static RoomImageDto Map(RoomImage image)
    {
        return new RoomImageDto(
            image.Id,
            image.RoomId,
            image.FileName,
            image.ContentType,
            image.SizeBytes,
            image.UploadedAtUtc);
    }
}
