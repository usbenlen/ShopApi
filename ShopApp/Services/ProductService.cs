using ShopApp.Interfaces;
using ShopDomain.Models;

namespace ShopApp.Services;

public class ProductService : IProductService
{
    private List<Product> _products = new();

    public void AddProduct(Product product)
    {
        _products.Add(product);
    }

    public List<Product> GetAllProducts()
    {
        _products.Add(new Product()
        {
            Title = "Cheese",
            Price = 41.4m
        });
        _products.Add(new Product()
        {
            Title = "Pie",
            Price = 64.9m
        });
        return _products;
    }
}
