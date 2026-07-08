using Shop.Application.DTOs.ProductDTOs;
using Shop.Application.Interfaces.Repository;
using Shop.Application.Interfaces.Services;
using Shop.Domain.Models;

namespace Shop.Application.Services;

/// <summary>
/// Сервіс для бізнес-логіки роботи з продуктами
/// </summary>
public class ProductService(IProductRepository _repository) : IProductService
{

    /// <inheritdoc/>
    public async Task<int?> CreateProductAsync(ProductCreateDTO dto)
    {
        Product product = new()
        {
            Name = dto.Name,
            Price = dto.Price,
            Description = dto.Description,
            StockQty = dto.StockQty,
            IsActive = dto.IsActive,
            CategoryId = dto.CategoryId,
            Images = dto.Images.Select(x => new ProductImage { Url = x }).ToList()
        };

        return await _repository.CreateProductAsync(product);
    }

    /// <inheritdoc/>
    public async Task<IReadOnlyList<ProductReadDTO>> GetProductsAsync()
    {
        var products = await _repository.GetProductsAsync();

        return products.Select(p => new ProductReadDTO
        {
            Id = p.Id,
            Name = p.Name,
            Price = p.Price,
            Description = p.Description,
            StockQty = p.StockQty,
            IsActive = p.IsActive,
            CategoryId = p.CategoryId,
            Images = p.Images.Select(i => i.Url).ToList()
        }).ToArray();
    }

    /// <inheritdoc/>
    public async Task<ProductReadDTO?> GetProductByIdAsync(int id)
    {
        var product = await _repository.GetProductByIdAsync(id);
        if (product == null) return null;

        return new ProductReadDTO
        {
            Id = product.Id,
            Name = product.Name,
            Price = product.Price,
            Description = product.Description,
            StockQty = product.StockQty,
            IsActive = product.IsActive,
            CategoryId = product.CategoryId,
            Images = product.Images.Select(i => i.Url).ToList()
        };
    }

    /// <inheritdoc/>
    public async Task<bool> DeleteProductAsync(int id)
    {
        return await _repository.DeleteProductAsync(id);
    }

    /// <inheritdoc/>
    public async Task<bool> UpdateProductAsync(int id, ProductUpdateDTO dto)
    {
        Product product = new()
        {
            Id = id,
            Name = dto.Name,
            Price = dto.Price,
            Description = dto.Description,
            StockQty = dto.StockQty,
            IsActive = dto.IsActive,
            CategoryId = dto.CategoryId,
            Images = dto.Images.Select(x => new ProductImage
                {
                    Url = x,
                    ProductId = id
                }).ToList()
        };

        return await _repository.UpdateProductAsync(product);
    }
}