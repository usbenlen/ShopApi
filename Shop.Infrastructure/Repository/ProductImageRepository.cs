using Microsoft.EntityFrameworkCore;

using Shop.Application.Interfaces.Repository;

using Shop.Domain.Models;

using Shop.Infrastructure.Data;

namespace Shop.Infrastructure.Repository;

public class ProductImageRepository(
    ShopDbContext context)
    : IProductImageRepository
{
    public async Task<IReadOnlyList<ProductImage>>GetByProductIdAsync(
            int productId,
            CancellationToken cancellationToken = default)
    {
        return await context.ProductImages
            .Where(x => x.ProductId == productId)
            .OrderBy(x => x.SortOrder)
            .ThenBy(x => x.Id)
            .AsNoTracking()
            .ToListAsync(cancellationToken);
    }

    public async Task<ProductImage?> GetAsync(
        int productId,
        int imageId,
        CancellationToken cancellationToken = default)
    {
        return await context.ProductImages
            .FirstOrDefaultAsync(
                x =>
                    x.ProductId == productId &&
                    x.Id == imageId,
                cancellationToken);
    }

    public async Task AddAsync(
        ProductImage image,
        CancellationToken cancellationToken = default)
    {
        await context.ProductImages.AddAsync(
            image,
            cancellationToken);
    }

    public Task DeleteAsync(
        ProductImage image,
        CancellationToken cancellationToken = default)
    {
        context.ProductImages.Remove(image);

        return Task.CompletedTask;
    }

    public async Task UnsetPrimaryAsync(
        int productId,
        CancellationToken cancellationToken = default)
    {
        await context.ProductImages
            .Where(x => x.ProductId == productId)
            .ExecuteUpdateAsync(
                setters => setters
                    .SetProperty(
                        x => x.IsPrimary,
                        false),
                cancellationToken);
    }

    public async Task UpdateOrderAsync(
        int productId,
        IReadOnlyList<int> imageIds,
        CancellationToken cancellationToken = default)
    {
        var images = await context.ProductImages
            .Where(x => x.ProductId == productId)
            .ToListAsync(cancellationToken);

        if (images.Count != imageIds.Count)
            return;

        var existingIds = images
            .Select(x => x.Id)
            .OrderBy(x => x)
            .ToList();

        var requestedIds = imageIds
            .OrderBy(x => x)
            .ToList();

        if (!existingIds.SequenceEqual(requestedIds))
            return;

        if (imageIds.Distinct().Count() != imageIds.Count)
            return;

        var orderMap = imageIds
            .Select((id, index) => new
            {
                Id = id,
                SortOrder = index
            })
            .ToDictionary(
                x => x.Id,
                x => x.SortOrder);

        foreach (var image in images)
        {
            image.SortOrder = orderMap[image.Id];
        }
    }

    public async Task SaveChangesAsync(
        CancellationToken cancellationToken = default)
    {
        await context.SaveChangesAsync(
            cancellationToken);
    }
}
