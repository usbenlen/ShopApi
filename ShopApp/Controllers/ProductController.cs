using Microsoft.AspNetCore.Mvc;
using Shop.Api.Filters;
using Shop.Api.Interfaces;
using Shop.Api.Requests.Products;
using Shop.Application.DTOs.ProductDTOs;
using Shop.Application.Interfaces.Services;

namespace Shop.Api.Controllers;

/// <summary>Контролер для роботи з продуктами</summary>
[ApiController]
[Route("api/[controller]")]
[LogActionFilter]
public class ProductsController(IProductService _productService, IImageService _imageService, IConfiguration _configuration) : ControllerBase
{
    /// <summary>Створити новий продукт разом із фотографіями</summary>
    /// <param name="dto">Дані продукту та файли зображень</param>
    /// <returns>Ідентифікатор створеного продукту</returns>
    [HttpPost]
    public async Task<IActionResult> Create([FromForm] ProductCreateRequest dto)
    {
        int maxImages = _configuration.GetValue<int>("ProductSettings:MaxImages");
        if (dto.ImagesFiles.Count > maxImages) return BadRequest($"Maximum allowed images: {maxImages}");

        dto.Images = [];

        foreach (var file in dto.ImagesFiles)
        {
            string? url = await _imageService.SaveFileAsync(file, _configuration["DirnameForFiles:Products"]);

            if (!string.IsNullOrEmpty(url)) dto.Images.Add(url);
        }

        var productDto = new ProductCreateDTO
        {
            Name = dto.Name,
            Price = dto.Price,
            Description = dto.Description,
            StockQty = dto.StockQty,
            IsActive = dto.IsActive,
            CategoryId = dto.CategoryId,
            Images = dto.Images
        };

        int? id = await _productService.CreateProductAsync(productDto);

        return Ok($"Product created {id}");
    }

    /// <summary>Отримати список усіх продуктів</summary>
    /// <returns>Список продуктів</returns>
    [HttpGet]
    public async Task<IActionResult> GetProducts()
    {
        var products = await _productService.GetProductsAsync();
        return Ok(products);
    }



    /// <summary>Отримати продукт за ID</summary>
    /// <param name="id">Ідентифікатор продукту</param>
    /// <returns>Продукт або NotFound</returns>
    [HttpGet("{id}")]
    public async Task<IActionResult> GetProductById(int id)
    {
        var product = await _productService.GetProductByIdAsync(id);
        if (product == null) return NotFound("Product not found");

        return Ok(product);
    }

    /// <summary>Оновити існуючий продукт</summary>
    /// <param name="id">Ідентифікатор продукту</param>
    /// <param name="dto">Нові дані продукту</param>
    /// <returns>Результат оновлення</returns>
    [HttpPut("{id}")]
    public async Task<IActionResult> UpdateProduct(int id, [FromForm] ProductUpdateRequest dto)
    {
        if (dto.ImagesFiles != null && dto.ImagesFiles.Count > 0)
        {
            dto.Images = [];

            foreach (var file in dto.ImagesFiles)
            {
                string? url = await _imageService.SaveFileAsync(file, _configuration["DirnameForFiles:Products"]);
                if (!string.IsNullOrEmpty(url)) dto.Images.Add(url);
            }
        }

        bool updated = await _productService.UpdateProductAsync(id, dto);
        if (!updated) return NotFound("Product not found");

        return Ok("Product updated");
    }

    /// <summary>Видалити продукт</summary>
    /// <param name="id">Ідентифікатор продукту</param>
    /// <returns>Результат видалення</returns>
    [HttpDelete("{id}")]
    public async Task<IActionResult> DeleteProduct(int id)
    {
        bool deleted = await _productService.DeleteProductAsync(id);
        if (!deleted) return NotFound("Product not found");

        return NoContent();
    }
}