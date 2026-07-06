using Shop.Api.Interfaces;
using Shop.Domain.Models;
using Shop.Application.DTOs.ProductDTOs;

namespace Shop.Api.Services;

public class ProductService : IProductService
{
    private readonly List<Product> _products = [
        new Product
        {
            Id = 1,
            Name = "Milk",
            Price = 40.2m
        },
        new Product
        {
            Id = 2,
            Name = "Bread",
            Price = 25.7m
        },
        new Product
        {
            Id = 3,
            Name = "Cheese",
            Price = 95.3m
        }
    ];

    public List<Product> GetAllProducts()
    {
        return _products;
    }

    public Product? GetById(int id)
    {
        return _products.FirstOrDefault(x => x.Id == id);
    }

    public Product Add(ProductDTO dto)
    {
        int id = _products.Max(x => x.Id) + 1;

        Product product = new Product
        {
            Id = id,
            Name = dto.Name,
            Price = dto.Price
        };  

        _products.Add(product);

        return product;
    }

    public Product? Update(int id, ProductDTO dto)
    {
        Product? product = GetById(id);
        if (product == null) return null;

        product.Name = dto.Name;
        product.Price = dto.Price;

        return product;
    }

    public bool Delete(int id)
    {
        Product? product = GetById(id);
        if (product == null) return false;

        _products.Remove(product);

        return true;
    }

    public List<Product> Search(string name)
    {
        return _products.Where(x => x.Name.ToLower().Contains(name.ToLower())).ToList();
    }
}
