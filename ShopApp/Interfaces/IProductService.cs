using Shop.Domain.Models;
using Shop.Application.DTOs.ProductDTOs;

namespace Shop.Api.Interfaces;

public interface IProductService
{
    List<Product> GetAllProducts();
    Product? GetById(int id);
    Product Add(ProductDTO dto);
    Product? Update(int id, ProductDTO dto);
    bool Delete(int id);
    List<Product> Search(string name);
}
