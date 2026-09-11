namespace Shop.Api.Interfaces;

public interface IImageService
{
    Task<string> SaveFileAsync(IFormFile file, string folderName, CancellationToken cancellationToken = default);
}
