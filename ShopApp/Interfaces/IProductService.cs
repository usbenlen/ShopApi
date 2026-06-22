using ShopDomain.Models;

namespace ShopApp.Interfaces;

public interface IProductService
{
    List<Product> GetAllProducts();
    void AddProduct(Product product);
}
