using Shop.Domain.Models;

namespace Shop.Application.Interfaces.Repository;

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

    Task UpdateOrderAsync(
        int productId,
        IReadOnlyList<int> imageIds,
        CancellationToken cancellationToken = default);

    Task SaveChangesAsync(
        CancellationToken cancellationToken = default);
}