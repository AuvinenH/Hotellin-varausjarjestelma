namespace HotelLakeview.Application.Abstractions;

public interface IRoomImageStorage
{
    Task SaveAsync(string storedFileName, Stream content, CancellationToken cancellationToken);

    Task<Stream> OpenReadAsync(string storedFileName, CancellationToken cancellationToken);

    Task DeleteAsync(string storedFileName, CancellationToken cancellationToken);
}
