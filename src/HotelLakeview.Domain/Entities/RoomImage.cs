namespace HotelLakeview.Domain.Entities;

public class RoomImage
{
    public RoomImage(Guid id, Guid roomId, string fileName, string storedFileName, string contentType, long sizeBytes)
    {
        if (string.IsNullOrWhiteSpace(fileName))
        {
            throw new ArgumentException("File name is required.");
        }

        if (string.IsNullOrWhiteSpace(storedFileName))
        {
            throw new ArgumentException("Stored file name is required.");
        }

        if (string.IsNullOrWhiteSpace(contentType))
        {
            throw new ArgumentException("Content type is required.");
        }

        if (sizeBytes <= 0)
        {
            throw new ArgumentException("Size must be greater than zero.");
        }

        Id = id;
        RoomId = roomId;
        FileName = fileName.Trim();
        StoredFileName = storedFileName.Trim();
        ContentType = contentType.Trim();
        SizeBytes = sizeBytes;
        UploadedAtUtc = DateTime.UtcNow;
    }

    private RoomImage()
    {
    }

    public Guid Id { get; private set; }

    public Guid RoomId { get; private set; }

    public string FileName { get; private set; } = string.Empty;

    public string StoredFileName { get; private set; } = string.Empty;

    public string ContentType { get; private set; } = string.Empty;

    public long SizeBytes { get; private set; }

    public DateTime UploadedAtUtc { get; private set; }
}
