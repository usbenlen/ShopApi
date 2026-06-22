using Microsoft.AspNetCore.Mvc;
using ShopDomain.Models;

namespace ShopApp.Controllers;

// https://localhost:port/product
// https://localhost:port/api/product
// ім'я product береться з ProductController тільки маленькими і без Controller
[ApiController]
[Route("api/[controller]")]
public class ProductController : ControllerBase
{
    private List<Product> _products = new();

    [HttpGet("get")] //https://localhost:port/api/product/get
    public List<Product> GetProducts()
    {
        _products.Add(new Product()
        {
            Title = "Cheese",
            Price = 55.34m
        });
        _products.Add(new Product()
        {
            Title = "Yogurt",
            Price = 42.52m
        });

        return _products;
    }
}
