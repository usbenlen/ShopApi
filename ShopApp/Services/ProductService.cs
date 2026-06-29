using ShopApp.Interfaces;
using ShopApp.Models;
using ShopDomain.Models;

namespace ShopApp.Services;

public class ProductService : IProductService
{
    private readonly List<Product> _products = [
        new Product
        {
            Id = 1,
            Title = "Milk",
            Price = 40.2m
        },
        new Product
        {
            Id = 2,
            Title = "Bread",
            Price = 25.7m
        },
        new Product
        {
            Id = 3,
            Title = "Cheese",
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
            Title = dto.Title,
            Price = dto.Price
        };

        _products.Add(product);

        return product;
    }

    public Product? Update(int id, ProductDTO dto)
    {
        Product? product = GetById(id);
        if (product == null) return null;

        product.Title = dto.Title;
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

    public List<Product> Search(string title)
    {
        return _products.Where(x => x.Title.ToLower().Contains(title.ToLower())).ToList();
    }
}
