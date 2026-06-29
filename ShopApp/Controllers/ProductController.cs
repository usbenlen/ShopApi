using Microsoft.AspNetCore.Mvc;
using Shop.Api.Filters;
using Shop.Api.Interfaces;
using Shop.Domain.Models;
using Shop.Domain.DTOs;

namespace Shop.Api.Controllers;

// https://localhost:port/products
// https://localhost:port/api/products
// ім'я products береться з ProductController тільки маленькими і без Controller

/// <summary>
/// Контролер для роботи з продуктами
/// </summary>
[ApiController]
[Route("api/[controller]")]
[LogActionFilter]
public class ProductsController(IProductService _productService) : ControllerBase
{
    /// <summary>
    /// Отримати список усіх продуктів
    /// </summary>
    [HttpGet] //https://localhost:port/api/products/
    [ProducesResponseType(StatusCodes.Status200OK)]
    public IActionResult GetProducts()
    {
        return Ok(_productService.GetAllProducts());
    }

    /// <summary>
    /// Отримати товар за ID
    /// </summary>
    /// <param name="id">Ідентифікатор продукту</param>
    [HttpGet("{id}")]
    [ProducesResponseType(typeof(Product), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public IActionResult Get(int id)
    {
        var product = _productService.GetById(id);
        if (product == null) return NotFound("Product not found");

        return Ok(product);
    }

    /// <summary>
    /// Додати новий продукт
    /// </summary>
    /// <param name="dto">Дані нового продукту</param>
    [HttpPost]
    [ProducesResponseType(typeof(Product), StatusCodes.Status201Created)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    public IActionResult Post(ProductDTO dto)
    {
        var product = _productService.Add(dto);

        return CreatedAtAction(nameof(Get), new { id = product.Id }, product);
    }

    /// <summary>
    /// Оновити існуючий продукт
    /// </summary>
    /// <param name="id">Ідентифікатор продукту</param>
    /// <param name="dto">Нові дані продукту</param>
    [HttpPut("{id}")]
    [ProducesResponseType(typeof(Product), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public IActionResult Put(int id, ProductDTO dto)
    {
        var product = _productService.Update(id, dto);
        if (product == null) return NotFound("Product not found");

        return Ok(product);
    }

    /// <summary>
    /// Видалити продукт
    /// </summary>
    /// <param name="id">Ідентифікатор продукту</param>
    [HttpDelete("{id}")]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public IActionResult Delete(int id)
    {
        bool deleted = _productService.Delete(id);
        if (!deleted) return NotFound("Product not found");

        return NoContent();
    }

    /// <summary>
    /// Пошук товарів за назвою
    /// </summary>
    /// <param name="title">Назва товару</param>
    [HttpGet("search")]
    [ProducesResponseType(typeof(List<Product>), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    public IActionResult Search(string? title)
    {
        if (string.IsNullOrWhiteSpace(title)) return BadRequest("Title is required");

        return Ok(_productService.Search(title));
    }
}
