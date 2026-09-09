using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Shop.Api.Filters;
using Shop.Api.Interfaces;
using Shop.Api.Requests.Products;
using Shop.Application.Commands.Products.CreateProduct;
using Shop.Application.Commands.Products.DeleteProduct;
using Shop.Application.DTOs.ProductDTOs;
using Shop.Application.DTOs.ProductFeedbackDTOs;
using Shop.Application.Interfaces.Services;
using Shop.Application.Queries.Product.GetProductById;
using Shop.Domain.Enums;
using System.Security.Claims;

namespace Shop.Api.Controllers;

/// <summary>Контролер для роботи з продуктами</summary>
[ApiController]
[Route("api/[controller]")]
public class ProductsController(IProductService _productService, IImageService _imageService, IProductFeedbackService _productFeedbackService, IConfiguration _configuration, IMediator _mediator) : ControllerBase
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

        int? id = await _mediator.Send(new CreateProductCommand(productDto));
        //int? id = await _productService.CreateProductAsync(productDto);

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
        var product = await _mediator.Send(new GetProductByIdQuery(id));
        //var product = await _productService.GetProductByIdAsync(id);
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
        var deleted = await _mediator.Send(new DeleteProductCommand(id));
        if (deleted is null) return NotFound("Product not found");
        //bool deleted = await _productService.DeleteProductAsync(id);
        //if (!deleted) return NotFound("Product not found");

        return NoContent();
    }

    /// <summary>
    /// Додати відгук або питання до продукту
    /// </summary>
    /// <param name="id">Ідентифікатор продукту</param>
    /// <param name="dto">Дані відгуку або питання</param>
    /// <returns>Результат створення</returns>
    [Authorize]
    [HttpPost("{id}/feedback")]
    public async Task<IActionResult> CreateFeedback(int id, [FromBody] ProductFeedbackCreateDTO dto)
    {
        if (id <= 0) return BadRequest("Invalid product id");

        if (string.IsNullOrWhiteSpace(dto.Message)) 
            return BadRequest("Message is required");

        if (dto.Message.Length > 2000) 
            return BadRequest("Message cannot exceed 2000 characters");

        if (dto.Type == ProductFeedbackType.Review)
        {
            if (!dto.Rating.HasValue || dto.Rating < 1 || dto.Rating > 5)
                return BadRequest("Review rating must be between 1 and 5");
        }
        else if (dto.Type == ProductFeedbackType.Question)
            dto.Rating = null;

        var userIdClaim = User.FindFirst(ClaimTypes.NameIdentifier);
        if (userIdClaim == null || !Guid.TryParse(userIdClaim.Value, out Guid userId))
            return Unauthorized();

        var product = await _productService.GetProductByIdAsync(id);
        if (product == null) return NotFound("Product not found");

        await _productFeedbackService.CreateAsync(id, userId, dto);

        return Ok("Feedback added");
    }

}