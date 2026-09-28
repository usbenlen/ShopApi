using AutoMapper;

using Shop.Application.DTOs.ProductDTOs;
using Shop.Application.Interfaces.Repository;
using Shop.Application.Interfaces.Services;

using Shop.Domain.Models;

namespace Shop.Infrastructure.Services;

public class ProductImageService(
    IProductImageRepository productImageRepository,
    IProductRepository productRepository,
    IImageService imageService,
    IMapper mapper)
    : IProductImageService
{
    public async Task<IReadOnlyList<ProductImageReadDTO>> GetByProductIdAsync(
            int productId,
            CancellationToken cancellationToken = default)
    {
        var images =
            await productImageRepository.GetByProductIdAsync(
                productId,
                cancellationToken);

        return mapper.Map<
            IReadOnlyList<ProductImageReadDTO>>(images);
    }

    public async Task<ProductImageReadDTO?> AddAsync(
        int productId,
        Stream stream,
        string fileName,
        string? contentType,
        CancellationToken cancellationToken = default)
    {
        var product = await productRepository.GetProductByIdAsync(
            productId,
            cancellationToken);

        if (product == null)
            return null;

        var existingImages =
            await productImageRepository.GetByProductIdAsync(
                productId,
                cancellationToken);

        var url = await imageService.SaveFileAsync(
            stream,
            fileName,
            contentType,
            "products",
            cancellationToken);

        if (string.IsNullOrWhiteSpace(url))
            throw new InvalidOperationException(
                "Не вдалося зберегти зображення");

        var sortOrder = existingImages.Count;

        var image = new ProductImage
        {
            Url = url,
            ProductId = productId,
            IsPrimary = existingImages.Count == 0,
            SortOrder = sortOrder
        };

        await productImageRepository.AddAsync(
            image,
            cancellationToken);

        await productImageRepository.SaveChangesAsync(
            cancellationToken);

        return mapper.Map<ProductImageReadDTO>(image);
    }

    public async Task<bool> DeleteAsync(
        int productId,
        int imageId,
        CancellationToken cancellationToken = default)
    {
        var image = await productImageRepository.GetAsync(
            productId,
            imageId,
            cancellationToken);

        if (image == null)
            return false;

        var wasPrimary = image.IsPrimary;
        var deletedSortOrder = image.SortOrder;

        await productImageRepository.DeleteAsync(
            image,
            cancellationToken);

        await productImageRepository.SaveChangesAsync(
            cancellationToken);

        await imageService.DeleteFileAsync(
            image.Url,
            cancellationToken);

        var remainingImages =
            await productImageRepository.GetByProductIdAsync(
                productId,
                cancellationToken);

        foreach (var remainingImage in remainingImages)
        {
            if (remainingImage.SortOrder > deletedSortOrder)
            {
                remainingImage.SortOrder--;
            }
        }

        if (wasPrimary && remainingImages.Count > 0)
        {
            await productImageRepository.UnsetPrimaryAsync(
                productId,
                cancellationToken);

            var newPrimary = remainingImages
                .OrderBy(x => x.SortOrder)
                .ThenBy(x => x.Id)
                .First();

            newPrimary.IsPrimary = true;
        }

        await productImageRepository.SaveChangesAsync(
            cancellationToken);

        return true;
    }

    public async Task<bool> SetPrimaryAsync(
        int productId,
        int imageId,
        CancellationToken cancellationToken = default)
    {
        var image = await productImageRepository.GetAsync(
            productId,
            imageId,
            cancellationToken);

        if (image == null)
            return false;

        await productImageRepository.UnsetPrimaryAsync(
            productId,
            cancellationToken);

        image.IsPrimary = true;

        await productImageRepository.SaveChangesAsync(
            cancellationToken);

        return true;
    }

    public async Task<bool> UpdateOrderAsync(
        int productId,
        IReadOnlyList<int> imageIds,
        CancellationToken cancellationToken = default)
    {
        if (imageIds == null || imageIds.Count == 0)
            return false;

        var images =
            await productImageRepository.GetByProductIdAsync(
                productId,
                cancellationToken);

        if (images.Count != imageIds.Count)
            return false;

        var existingIds = images
            .Select(x => x.Id)
            .OrderBy(x => x)
            .ToList();

        var requestedIds = imageIds
            .OrderBy(x => x)
            .ToList();

        if (!existingIds.SequenceEqual(requestedIds))
            return false;

        if (imageIds.Distinct().Count() != imageIds.Count)
            return false;

        await productImageRepository.UpdateOrderAsync(
            productId,
            imageIds,
            cancellationToken);

        await productImageRepository.UnsetPrimaryAsync(
            productId,
            cancellationToken);

        var firstImageId = imageIds[0];

        var primaryImage =
            await productImageRepository.GetAsync(
                productId,
                firstImageId,
                cancellationToken);

        if (primaryImage == null)
            return false;

        primaryImage.IsPrimary = true;

        await productImageRepository.SaveChangesAsync(
            cancellationToken);

        return true;
    }
}
