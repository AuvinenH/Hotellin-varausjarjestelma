using HotelLakeview.Application.Abstractions;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Hosting;

namespace HotelLakeview.Infrastructure.Storage;

public class LocalRoomImageStorage : IRoomImageStorage
{
    private readonly string _rootPath;

    public LocalRoomImageStorage(IConfiguration configuration, IHostEnvironment hostEnvironment)
    {
        var configuredPath = configuration["Storage:RoomImagesPath"] ?? Path.Combine("uploads", "rooms");
        _rootPath = Path.Combine(hostEnvironment.ContentRootPath, configuredPath);
        Directory.CreateDirectory(_rootPath);
    }

    public async Task SaveAsync(string storedFileName, Stream content, CancellationToken cancellationToken)
    {
        var fullPath = ResolveFullPath(storedFileName);
        var directoryPath = Path.GetDirectoryName(fullPath);

        if (!string.IsNullOrWhiteSpace(directoryPath))
        {
            Directory.CreateDirectory(directoryPath);
        }

        await using var fileStream = new FileStream(fullPath, FileMode.Create, FileAccess.Write, FileShare.None);
        await content.CopyToAsync(fileStream, cancellationToken);
    }

    public Task<Stream> OpenReadAsync(string storedFileName, CancellationToken cancellationToken)
    {
        var fullPath = ResolveFullPath(storedFileName);

        if (!File.Exists(fullPath))
        {
            throw new FileNotFoundException("Image file was not found on disk.", fullPath);
        }

        Stream stream = new FileStream(fullPath, FileMode.Open, FileAccess.Read, FileShare.Read);
        return Task.FromResult(stream);
    }

    public Task DeleteAsync(string storedFileName, CancellationToken cancellationToken)
    {
        var fullPath = ResolveFullPath(storedFileName);

        if (File.Exists(fullPath))
        {
            File.Delete(fullPath);
        }

        return Task.CompletedTask;
    }

    private string ResolveFullPath(string storedFileName)
    {
        var sanitizedRelativePath = storedFileName.Replace('/', Path.DirectorySeparatorChar);
        var candidatePath = Path.GetFullPath(Path.Combine(_rootPath, sanitizedRelativePath));
        var normalizedRootPath = Path.GetFullPath(_rootPath);

        if (!candidatePath.StartsWith(normalizedRootPath, StringComparison.OrdinalIgnoreCase))
        {
            throw new InvalidOperationException("Invalid image path.");
        }

        return candidatePath;
    }
}
