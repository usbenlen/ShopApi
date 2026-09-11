using AutoMapper;
using Microsoft.Extensions.Options;
using Shop.Application.Configuration;
using Shop.Application.Constants;
using Shop.Application.DTOs.ProductDTOs;
using Shop.Application.Interfaces.Repository;
using Shop.Application.Interfaces.Services;
using Shop.Domain.Models;

namespace Shop.Application.Services;

/// <summary>
/// Сервіс для бізнес-логіки роботи з продуктами
/// </summary>
public class ProductService(IProductRepository _repository, IMapper _mapper, ICachingService _cache, IOptions<CachingSettings> _cacheSettings) : IProductService
{
    private readonly TimeSpan CacheExpiration = TimeSpan.FromMinutes(_cacheSettings.Value.Products.ExpirationMinutes);

    /// <inheritdoc/>
    public async Task<int?> CreateProductAsync(ProductCreateDTO dto, CancellationToken cancellationToken = default)
    {
        Product product = _mapper.Map<Product>(dto);

        product.Images = dto.Images.Select(x => new ProductImage
        {
            Url = x
        }).ToList();

        var result = await _repository.CreateProductAsync(product, cancellationToken);

        if (result.HasValue)
        {
            await _cache.InvalidateGroupAsync(CacheKeys.ProductsGroup, cancellationToken);
            await _cache.InvalidateGroupAsync(CacheKeys.CategoriesGroup, cancellationToken);
        }

        return result;
    }

    /// <inheritdoc/>
    public async Task<IReadOnlyList<ProductReadDTO>> GetProductsAsync(CancellationToken cancellationToken = default)
    {
        return await _cache.GetOrCreateAsync(
            CacheKeys.AllProducts,

            async ct =>
            {
                var products = await _repository.GetProductsAsync(ct);
                return _mapper.Map<List<ProductReadDTO>>(products);
            },

            CacheExpiration,
            CacheKeys.ProductsGroup,
            cancellationToken) ?? [];
    }

    /// <inheritdoc/>
    public async Task<ProductReadDTO?> GetProductByIdAsync(int id, CancellationToken cancellationToken = default)
    {
        return await _cache.GetOrCreateAsync(
            CacheKeys.Product(id),

            async ct =>
            {
                var product = await _repository.GetProductByIdAsync(id, ct);
                if (product is null) return null;
                return _mapper.Map<ProductReadDTO>(product);
            },

            CacheExpiration,
            CacheKeys.ProductsGroup,
            cancellationToken);
    }

    /// <inheritdoc/>
    public async Task<bool> DeleteProductAsync(int id, CancellationToken cancellationToken = default)
    {
        var result = await _repository.DeleteProductAsync(id, cancellationToken);

        if (result)
        {
            await _cache.InvalidateGroupAsync(CacheKeys.ProductsGroup, cancellationToken);
            await _cache.InvalidateGroupAsync(CacheKeys.CategoriesGroup, cancellationToken);
        }

        return result;
    }

    /// <inheritdoc/>
    public async Task<bool> UpdateProductAsync(int id, ProductUpdateDTO dto, CancellationToken cancellationToken = default)
    {
        var product = await _repository.GetProductForUpdateAsync(id, cancellationToken);

        if (product is null) return false;

        _mapper.Map(dto, product);

        if (dto.Images != null)
        {
            product.Images.Clear();

            foreach (var image in dto.Images)
                product.Images.Add(new ProductImage
                {
                    Url = image,
                    ProductId = id
                });
        }

        var result = await _repository.UpdateProductAsync(cancellationToken);

        if (result)
        {
            await _cache.InvalidateGroupAsync(CacheKeys.ProductsGroup, cancellationToken);
            await _cache.InvalidateGroupAsync(CacheKeys.CategoriesGroup, cancellationToken);
        }

        return result;
    }
}