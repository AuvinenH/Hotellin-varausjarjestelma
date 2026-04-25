using HotelLakeview.Application.Abstractions;
using HotelLakeview.Application.Contracts.Images;
using HotelLakeview.Application.Exceptions;
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

    public async Task<IReadOnlyList<RoomImageDto>> GetByRoomIdAsync(Guid roomId, CancellationToken cancellationToken)
    {
        await EnsureRoomExists(roomId, cancellationToken);
        var images = await _imageRepository.GetByRoomIdAsync(roomId, cancellationToken);
        return images.Select(Map).ToList();
    }

    public async Task<RoomImageDto> UploadAsync(Guid roomId, string fileName, string contentType, long sizeBytes, Stream content, CancellationToken cancellationToken)
    {
        await EnsureRoomExists(roomId, cancellationToken);

        if (string.IsNullOrWhiteSpace(fileName))
        {
            throw new ArgumentException("File name is required.");
        }

        if (!AllowedContentTypes.Contains(contentType))
        {
            throw new ArgumentException("Unsupported file type.");
        }

        if (sizeBytes <= 0 || sizeBytes > MaxFileSizeBytes)
        {
            throw new ArgumentException("Image size must be between 1 byte and 5 MB.");
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

        return Map(image);
    }

    public async Task<(Stream Content, string ContentType, string FileName)> OpenImageAsync(Guid roomId, Guid imageId, CancellationToken cancellationToken)
    {
        await EnsureRoomExists(roomId, cancellationToken);

        var image = await _imageRepository.GetByIdAsync(imageId, cancellationToken)
            ?? throw new NotFoundException($"Image '{imageId}' was not found.");

        if (image.RoomId != roomId)
        {
            throw new NotFoundException($"Image '{imageId}' was not found for room '{roomId}'.");
        }

        var stream = await _storage.OpenReadAsync(image.StoredFileName, cancellationToken);
        return (stream, image.ContentType, image.FileName);
    }

    public async Task DeleteAsync(Guid roomId, Guid imageId, CancellationToken cancellationToken)
    {
        await EnsureRoomExists(roomId, cancellationToken);

        var image = await _imageRepository.GetByIdAsync(imageId, cancellationToken)
            ?? throw new NotFoundException($"Image '{imageId}' was not found.");

        if (image.RoomId != roomId)
        {
            throw new NotFoundException($"Image '{imageId}' was not found for room '{roomId}'.");
        }

        await _storage.DeleteAsync(image.StoredFileName, cancellationToken);
        _imageRepository.Remove(image);
        await _imageRepository.SaveChangesAsync(cancellationToken);
    }

    private async Task EnsureRoomExists(Guid roomId, CancellationToken cancellationToken)
    {
        _ = await _roomRepository.GetByIdAsync(roomId, cancellationToken)
            ?? throw new NotFoundException($"Room '{roomId}' was not found.");
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
