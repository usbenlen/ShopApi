using AutoMapper;
using MediatR;
using Shop.Application.Commands.Products.CreateProduct;
using Shop.Application.Constants;
using Shop.Application.Interfaces.Repository;
using Shop.Application.Interfaces.Services;
using Shop.Domain.Models;

namespace Shop.Application.Commands.Products.CreateProduct;

public class CreateProductHandler(
    IProductRepository productRepository,
    IMapper mapper,
    ICachingService cache)
    : IRequestHandler<CreateProductCommand, int?>
{
    public async Task<int?> Handle(CreateProductCommand request, CancellationToken cancellationToken)
    {
        var product = mapper.Map<Product>(request.DTO);

        product.Images = request.DTO.Images
            .Select(x => new ProductImage
            {
                Url = x
            })
            .ToList();

        var result = await productRepository.CreateProductAsync(product, cancellationToken);

        if (result.HasValue)
        {
            await cache.InvalidateGroupAsync(CacheKeys.ProductsGroup, cancellationToken);
            await cache.InvalidateGroupAsync(CacheKeys.CategoriesGroup, cancellationToken);
        }

        return result;
    }
}
