namespace Shop.Application.Interfaces.Services;

public interface IImageService
{
    Task<string?> SaveFileAsync(
        Stream stream,
        string fileName,
        string? contentType,
        string directory,
        CancellationToken cancellationToken = default);

    Task DeleteFileAsync(
        string? url,
        CancellationToken cancellationToken = default);
}