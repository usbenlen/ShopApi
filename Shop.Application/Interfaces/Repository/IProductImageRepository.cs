using Shop.Domain.Models;

public interface IProductImageRepository
{
    Task<IReadOnlyList<ProductImage>> GetByProductIdAsync(
        int productId,
        CancellationToken cancellationToken = default);

    Task<ProductImage?> GetAsync(
        int productId,
        int imageId,
        CancellationToken cancellationToken = default);

    Task AddAsync(
        ProductImage image,
        CancellationToken cancellationToken = default);

    Task DeleteAsync(
        ProductImage image,
        CancellationToken cancellationToken = default);

    Task UnsetPrimaryAsync(
        int productId,
        CancellationToken cancellationToken = default);

    Task SaveChangesAsync(
        CancellationToken cancellationToken = default);
}
