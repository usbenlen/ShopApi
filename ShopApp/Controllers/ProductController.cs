using Microsoft.AspNetCore.Mvc;
using ShopApp.Interfaces;
using ShopDomain.Models;

namespace ShopApp.Controllers;

// https://localhost:port/product
// https://localhost:port/api/product
// ім'я product береться з ProductController тільки маленькими і без Controller
[ApiController]
[Route("api/[controller]")]
public class ProductController(IProductService _productService) : ControllerBase
{
    //private List<Product> _products = new();

    [HttpGet("get")] //https://localhost:port/api/product/get
    public IActionResult GetProducts()
    {
        return Ok(_productService.GetAllProducts());
    }

    [HttpPost]
    public IActionResult AddNewProduct([FromBody] Product product)
    {
        _productService.AddProduct(product);
        return Ok("Product added successfully");
    }
}
