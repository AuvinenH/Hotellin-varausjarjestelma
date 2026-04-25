namespace HotelLakeview.Application.Contracts.Images;

public sealed record RoomImageDto(
    Guid Id,
    Guid RoomId,
    string FileName,
    string ContentType,
    long SizeBytes,
    DateTime UploadedAtUtc);
