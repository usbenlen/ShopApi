using Shop.Api.Interfaces;

namespace Shop.Api.Services;

public class ImageService(IWebHostEnvironment _environment) : IImageService
{
    public async Task<string> SaveFileAsync(IFormFile file, string folderName)
    {
        if (file == null || file.Length == 0) throw new ArgumentException("File is empty");

        var folderPath = Path.Combine(_environment.WebRootPath, folderName);
        Directory.CreateDirectory(folderPath);

        // Унікальна назва файлу
        var fileName = $"{Guid.NewGuid()}{Path.GetExtension(file.FileName)}";
        var filePath = Path.Combine(folderPath, fileName);

        await using var stream = new FileStream(filePath, FileMode.Create);
        await file.CopyToAsync(stream);

        return $"{fileName}";
    }
}
