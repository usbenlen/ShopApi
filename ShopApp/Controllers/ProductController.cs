using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

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

/// <summary>
/// Контролер для роботи з продуктами
/// </summary>
[ApiController]
[Route("api/[controller]")]
public class ProductsController(
    IProductService productService,
    IImageService imageService,
    IProductFeedbackService productFeedbackService,
    IConfiguration configuration,
    IMediator mediator)
    : ControllerBase
{
    /// <summary>
    /// Створити новий продукт разом із фотографіями
    /// </summary>
    [HttpPost]
    public async Task<IActionResult> Create(
        [FromForm] ProductCreateRequest dto,
        CancellationToken cancellationToken)
    {
        var maxImages = configuration.GetValue<int>(
            "ProductSettings:MaxImages");

        if (dto.ImagesFiles.Count > maxImages)
        {
            return BadRequest(
                $"Maximum allowed images: {maxImages}");
        }

        dto.Images = [];

        foreach (var file in dto.ImagesFiles)
        {
            if (file == null || file.Length == 0)
                continue;

            if (string.IsNullOrWhiteSpace(file.ContentType) ||
                !file.ContentType.StartsWith(
                    "image/",
                    StringComparison.OrdinalIgnoreCase))
            {
                return BadRequest(
                    $"File '{file.FileName}' is not an image.");
            }

            await using var stream = file.OpenReadStream();

            var url = await imageService.SaveFileAsync(
                stream,
                file.FileName,
                file.ContentType,
                "products",
                cancellationToken);

            if (!string.IsNullOrWhiteSpace(url))
            {
                dto.Images.Add(url);
            }
        }

        var productDto = new ProductCreateDTO
        {
            Name = dto.Name,
            Price = dto.Price,
            OldPrice = dto.OldPrice,
            IsDiscounted = dto.IsDiscounted,
            Description = dto.Description,
            StockQty = dto.StockQty,
            IsActive = dto.IsActive,
            CategoryId = dto.CategoryId,
            Images = dto.Images
        };

        var id = await mediator.Send(
            new CreateProductCommand(productDto),
            cancellationToken);

        return Ok($"Product created {id}");
    }

    /// <summary>
    /// Отримати список усіх продуктів
    /// </summary>
    [HttpGet]
    public async Task<IActionResult> GetProducts(
        CancellationToken cancellationToken)
    {
        var products = await productService.GetProductsAsync(
            cancellationToken);

        return Ok(products);
    }

    /// <summary>
    /// Отримати продукт за ID
    /// </summary>
    [HttpGet("{id:int}")]
    public async Task<IActionResult> GetProductById(
        int id,
        CancellationToken cancellationToken)
    {
        var product = await mediator.Send(
            new GetProductByIdQuery(id),
            cancellationToken);

        if (product == null)
            return NotFound("Product not found");

        return Ok(product);
    }

    /// <summary>
    /// Оновити існуючий продукт
    /// </summary>
    [HttpPut("{id:int}")]
    public async Task<IActionResult> UpdateProduct(
        int id,
        [FromForm] ProductUpdateRequest dto,
        CancellationToken cancellationToken)
    {
        var productDto = new ProductUpdateDTO
        {
            Name = dto.Name,
            Price = dto.Price,
            OldPrice = dto.OldPrice,
            IsDiscounted = dto.IsDiscounted,
            Description = dto.Description,
            StockQty = dto.StockQty,
            IsActive = dto.IsActive,
            CategoryId = dto.CategoryId
        };

        var updated = await productService.UpdateProductAsync(
            id,
            productDto,
            cancellationToken);

        if (!updated)
            return NotFound("Product not found");

        return Ok("Product updated");
    }

    /// <summary>
    /// Видалити продукт
    /// </summary>
    [HttpDelete("{id:int}")]
    public async Task<IActionResult> DeleteProduct(
        int id,
        CancellationToken cancellationToken)
    {
        var deleted = await mediator.Send(
            new DeleteProductCommand(id),
            cancellationToken);

        if (deleted is null)
            return NotFound("Product not found");

        return NoContent();
    }

    /// <summary>
    /// Додати відгук або питання до продукту
    /// </summary>
    [Authorize]
    [HttpPost("{id:int}/feedback")]
    public async Task<IActionResult> CreateFeedback(
        int id,
        [FromBody] ProductFeedbackCreateDTO dto,
        CancellationToken cancellationToken)
    {
        if (id <= 0)
            return BadRequest("Invalid product id");

        if (string.IsNullOrWhiteSpace(dto.Message))
            return BadRequest("Message is required");

        if (dto.Message.Length > 2000)
            return BadRequest(
                "Message cannot exceed 2000 characters");

        if (dto.Type == ProductFeedbackType.Review)
        {
            if (!dto.Rating.HasValue ||
                dto.Rating < 1 ||
                dto.Rating > 5)
            {
                return BadRequest(
                    "Review rating must be between 1 and 5");
            }
        }
        else if (dto.Type == ProductFeedbackType.Question)
        {
            dto.Rating = null;
        }

        var userIdClaim = User.FindFirst(
            ClaimTypes.NameIdentifier);

        if (userIdClaim == null ||
            !Guid.TryParse(
                userIdClaim.Value,
                out var userId))
        {
            return Unauthorized();
        }

        var product = await productService.GetProductByIdAsync(
            id,
            cancellationToken);

        if (product == null)
            return NotFound("Product not found");

        await productFeedbackService.CreateAsync(
            id,
            userId,
            dto,
            cancellationToken);

        return Ok("Feedback added");
    }
}
