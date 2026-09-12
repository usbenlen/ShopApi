using Shop.Application.DTOs.ProductDTOs;

public interface IProductImageService
{
    Task<IReadOnlyList<ProductImageReadDTO>> GetByProductIdAsync(
        int productId,
        CancellationToken cancellationToken = default);

    Task<ProductImageReadDTO?> AddAsync(
        int productId,
        IFormFile file,
        CancellationToken cancellationToken = default);

    Task<bool> DeleteAsync(
        int productId,
        int imageId,
        CancellationToken cancellationToken = default);

    Task<bool> SetPrimaryAsync(
        int productId,
        int imageId,
        CancellationToken cancellationToken = default);
}
