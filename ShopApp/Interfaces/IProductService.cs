using ShopApp.Models;
using ShopDomain.Models;

namespace ShopApp.Interfaces;

public interface IProductService
{
    List<Product> GetAllProducts();
    Product? GetById(int id);
    Product Add(ProductDTO dto);
    Product? Update(int id, ProductDTO dto);
    bool Delete(int id);
    List<Product> Search(string title);
}
