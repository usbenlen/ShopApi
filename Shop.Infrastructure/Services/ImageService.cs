using Microsoft.AspNetCore.Hosting;
using Shop.Application.Interfaces.Services;

namespace Shop.Infrastructure.Services;

public class ImageService(IWebHostEnvironment environment) : IImageService
{
    public async Task<string?> SaveFileAsync(
        Stream stream,
        string fileName,
        string? contentType,
        string directory,
        CancellationToken cancellationToken = default)
    {
        if (stream == null || stream.Length == 0)
            return null;

        var extension = Path.GetExtension(fileName);

        if (string.IsNullOrWhiteSpace(extension))
            return null;

        var uploadsDirectory = Path.Combine(
            environment.WebRootPath ?? Path.Combine(environment.ContentRootPath, "wwwroot"),
            directory);

        Directory.CreateDirectory(uploadsDirectory);

        var newFileName =
            $"{Guid.NewGuid():N}{extension.ToLowerInvariant()}";

        var filePath = Path.Combine(
            uploadsDirectory,
            newFileName);

        await using var fileStream = new FileStream(
            filePath,
            FileMode.Create,
            FileAccess.Write,
            FileShare.None,
            81920,
            useAsync: true);

        await stream.CopyToAsync(
            fileStream,
            cancellationToken);

        return $"/{directory.Trim('/')}/{newFileName}";
    }

    public Task DeleteFileAsync(
        string? url,
        CancellationToken cancellationToken = default)
    {
        if (string.IsNullOrWhiteSpace(url))
            return Task.CompletedTask;

        var relativePath = url
            .TrimStart('/')
            .Replace(
                '/',
                Path.DirectorySeparatorChar);

        var webRootPath =
            environment.WebRootPath ??
            Path.Combine(environment.ContentRootPath, "wwwroot");

        var filePath = Path.Combine(
            webRootPath,
            relativePath);

        if (File.Exists(filePath))
            File.Delete(filePath);

        return Task.CompletedTask;
    }
}
