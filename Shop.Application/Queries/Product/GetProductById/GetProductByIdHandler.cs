using AutoMapper;
using MediatR;
using Microsoft.Extensions.Options;
using Shop.Application.Configuration;
using Shop.Application.Constants;
using Shop.Application.DTOs.ProductDTOs;
using Shop.Application.Interfaces.Repository;
using Shop.Application.Interfaces.Services;

namespace Shop.Application.Queries.Product.GetProductById;

public class GetProductByIdHandler(
    IProductRepository _productRepository,
    IMapper _mapper,
    ICachingService _cache,
    IOptions<CachingSettings> _cacheSettings)
    : IRequestHandler<GetProductByIdQuery, ProductReadDTO?>
{
    private readonly TimeSpan _cacheExpiration = TimeSpan.FromMinutes(_cacheSettings.Value.Products.ExpirationMinutes);

    public async Task<ProductReadDTO?> Handle(GetProductByIdQuery request, CancellationToken cancellationToken)
    {
        return await _cache.GetOrCreateAsync(
            CacheKeys.Product(request.id),

            async ct =>
            {
                var product = await _productRepository.GetProductByIdAsync(request.id, ct);
                if (product is null) return null;

                return _mapper.Map<ProductReadDTO>(product);
            },

            _cacheExpiration,
            CacheKeys.ProductsGroup,
            cancellationToken);
    }
}
