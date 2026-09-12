using AutoMapper;
using Shop.Application.DTOs.ProductDTOs;
using Shop.Application.Interfaces.Repository;
using Shop.Domain.Models;

public class ProductImageService(
    IProductImageRepository repository,
    IProductRepository productRepository,
    IImageService imageService,
    IMapper mapper,
    IConfiguration configuration)
    : IProductImageService
{
    private const string ProductDirectory = "Products";

    public async Task<IReadOnlyList<ProductImageReadDTO>> GetByProductIdAsync(
        int productId,
        CancellationToken cancellationToken = default)
    {
        var images = await repository.GetByProductIdAsync(
            productId,
            cancellationToken);

        return mapper.Map<List<ProductImageReadDTO>>(images);
    }

    public async Task<ProductImageReadDTO?> AddAsync(
        int productId,
        IFormFile file,
        CancellationToken cancellationToken = default)
    {
        var product = await productRepository.GetProductByIdAsync(
            productId,
            cancellationToken);

        if (product == null)
            return null;

        var images = await repository.GetByProductIdAsync(
            productId,
            cancellationToken);

        var maxImages = configuration.GetValue<int>(
            "ProductSettings:MaxImages");

        if (images.Count >= maxImages)
            throw new InvalidOperationException(
                $"Maximum allowed images: {maxImages}");

        var url = await imageService.SaveFileAsync(
            file,
            configuration["DirnameForFiles:Products"],
            cancellationToken);

        if (string.IsNullOrWhiteSpace(url))
            throw new InvalidOperationException(
                "Image could not be saved");

        var image = new ProductImage
        {
            ProductId = productId,
            Url = url,
            IsPrimary = images.Count == 0
        };

        await repository.AddAsync(image, cancellationToken);
        await repository.SaveChangesAsync(cancellationToken);

        return mapper.Map<ProductImageReadDTO>(image);
    }

    public async Task<bool> DeleteAsync(
        int productId,
        int imageId,
        CancellationToken cancellationToken = default)
    {
        var image = await repository.GetAsync(
            productId,
            imageId,
            cancellationToken);

        if (image == null)
            return false;

        await repository.DeleteAsync(image, cancellationToken);
        await repository.SaveChangesAsync(cancellationToken);

        // Тут додаси фізичне видалення файлу.
        // await imageService.DeleteFileAsync(image.Url, cancellationToken);

        return true;
    }

    public async Task<bool> SetPrimaryAsync(
        int productId,
        int imageId,
        CancellationToken cancellationToken = default)
    {
        var image = await repository.GetAsync(
            productId,
            imageId,
            cancellationToken);

        if (image == null)
            return false;

        await repository.UnsetPrimaryAsync(
            productId,
            cancellationToken);

        image.IsPrimary = true;

        await repository.SaveChangesAsync(cancellationToken);

        return true;
    }
}
