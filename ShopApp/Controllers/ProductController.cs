using Microsoft.AspNetCore.Mvc;
using ShopApp.Filters;
using ShopApp.Interfaces;
using ShopApp.Models;
using ShopDomain.Models;

namespace ShopApp.Controllers;

// https://localhost:port/products
// https://localhost:port/api/products
// ім'я products береться з ProductController тільки маленькими і без Controller
[ApiController]
[Route("api/[controller]")]
[LogActionFilter]
public class ProductsController(IProductService _productService) : ControllerBase
{
    [HttpGet] //https://localhost:port/api/products/
    public IActionResult GetProducts()
    {
        return Ok(_productService.GetAllProducts());
    }

    [HttpGet("{id}")]
    public IActionResult Get(int id)
    {
        var product = _productService.GetById(id);
        if (product == null) return NotFound("Product not found");

        return Ok(product);
    }

    [HttpPost]
    public IActionResult Post(ProductDTO dto)
    {
        var product = _productService.Add(dto);

        return CreatedAtAction(nameof(Get), new { id = product.Id }, product);
    }

    [HttpPut("{id}")]
    public IActionResult Put(int id, ProductDTO dto)
    {
        var product = _productService.Update(id, dto);
        if (product == null) return NotFound("Product not found");

        return Ok(product);
    }

    [HttpDelete("{id}")]
    public IActionResult Delete(int id)
    {
        bool deleted = _productService.Delete(id);
        if (!deleted) return NotFound("Product not found");

        return NoContent();
    }

    [HttpGet("search")]
    public IActionResult Search(string? title)
    {
        if (string.IsNullOrWhiteSpace(title)) return BadRequest("Title is required");

        return Ok(_productService.Search(title));
    }
}
