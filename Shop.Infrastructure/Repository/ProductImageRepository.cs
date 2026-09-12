using Shop.Domain.Models;
using Shop.Infrastructure.Data;

public class ProductImageRepository(ShopDbContext context)
    : IProductImageRepository
{
    public async Task<IReadOnlyList<ProductImage>> GetByProductIdAsync(
        int productId,
        CancellationToken cancellationToken = default)
    {
        return await context.ProductImages
            .Where(x => x.ProductId == productId)
            .OrderByDescending(x => x.IsPrimary)
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
                x => x.ProductId == productId && x.Id == imageId,
                cancellationToken);
    }

    public async Task AddAsync(
        ProductImage image,
        CancellationToken cancellationToken = default)
    {
        await context.ProductImages.AddAsync(image, cancellationToken);
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
                    .SetProperty(x => x.IsPrimary, false),
                cancellationToken);
    }

    public async Task SaveChangesAsync(
        CancellationToken cancellationToken = default)
    {
        await context.SaveChangesAsync(cancellationToken);
    }
}
