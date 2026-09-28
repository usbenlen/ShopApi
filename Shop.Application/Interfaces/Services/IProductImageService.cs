using Shop.Application.DTOs.ProductDTOs;

namespace Shop.Application.Interfaces.Services;

public interface IProductImageService
{
    Task<IReadOnlyList<ProductImageReadDTO>> GetByProductIdAsync(
        int productId,
        CancellationToken cancellationToken = default);

    Task<ProductImageReadDTO?> AddAsync(
        int productId,
        Stream stream,
        string fileName,
        string? contentType,
        CancellationToken cancellationToken = default);

    Task<bool> DeleteAsync(
        int productId,
        int imageId,
        CancellationToken cancellationToken = default);

    Task<bool> SetPrimaryAsync(
        int productId,
        int imageId,
        CancellationToken cancellationToken = default);

    Task<bool> UpdateOrderAsync(
        int productId,
        IReadOnlyList<int> imageIds,
        CancellationToken cancellationToken = default);
}