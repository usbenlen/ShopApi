using AutoMapper;
using Shop.Application.DTOs.ProductDTOs;
using Shop.Application.Interfaces.Repository;
using Shop.Application.Interfaces.Services;
using Shop.Domain.Models;

namespace Shop.Application.Services;

/// <summary>
/// Сервіс для бізнес-логіки роботи з продуктами
/// </summary>
public class ProductService(IProductRepository _repository, IMapper _mapper) : IProductService
{

    /// <inheritdoc/>
    public async Task<int?> CreateProductAsync(ProductCreateDTO dto)
    {

        Console.WriteLine(dto.GetType().FullName);
        Product product = _mapper.Map<Product>(dto);

        product.Images = dto.Images.Select(x => new ProductImage
            {
                Url = x
            }).ToList();

        return await _repository.CreateProductAsync(product);
    }

    /// <inheritdoc/>
    public async Task<IReadOnlyList<ProductReadDTO>> GetProductsAsync()
    {
        var products = await _repository.GetProductsAsync();

        return _mapper.Map<List<ProductReadDTO>>(products);
    }

    /// <inheritdoc/>
    public async Task<ProductReadDTO?> GetProductByIdAsync(int id)
    {
        var product = await _repository.GetProductByIdAsync(id);
        if (product == null) return null;

        return _mapper.Map<ProductReadDTO>(product);
    }

    /// <inheritdoc/>
    public async Task<bool> DeleteProductAsync(int id)
    {
        return await _repository.DeleteProductAsync(id);
    }

    /// <inheritdoc/>
    public async Task<bool> UpdateProductAsync(int id, ProductUpdateDTO dto)
    {
        var product = await _repository.GetProductForUpdateAsync(id);
        if (product == null) return false;

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

        return await _repository.UpdateProductAsync(product);
    }
}