using Microsoft.AspNetCore.Mvc;
using Shop.Application.DTOs.ProductImageDTOs;
using Shop.Application.Interfaces.Services;

namespace Shop.Api.Controllers;

[ApiController]
[Route("api/products/{productId:int}/images")]
public class ProductImagesController(
    IProductImageService productImageService)
    : ControllerBase
{
    [HttpGet]
    public async Task<IActionResult> GetImages(
        int productId,
        CancellationToken cancellationToken)
    {
        var images = await productImageService.GetByProductIdAsync(
            productId,
            cancellationToken);

        return Ok(images);
    }

    [HttpPost]
    [RequestSizeLimit(10 * 1024 * 1024)]
    public async Task<IActionResult> AddImage(
        int productId,
        IFormFile file,
        CancellationToken cancellationToken)
    {
        if (file == null || file.Length == 0)
            return BadRequest("Image is required");

        if (string.IsNullOrWhiteSpace(file.ContentType) ||
            !file.ContentType.StartsWith(
                "image/",
                StringComparison.OrdinalIgnoreCase))
        {
            return BadRequest("Only image files are allowed");
        }

        try
        {
            await using var stream = file.OpenReadStream();

            var image = await productImageService.AddAsync(
                productId,
                stream,
                file.FileName,
                file.ContentType,
                cancellationToken);

            if (image == null)
                return NotFound("Product not found");

            return Ok(image);
        }
        catch (InvalidOperationException ex)
        {
            return BadRequest(ex.Message);
        }
    }

    [HttpDelete("{imageId:int}")]
    public async Task<IActionResult> DeleteImage(
        int productId,
        int imageId,
        CancellationToken cancellationToken)
    {
        var deleted = await productImageService.DeleteAsync(
            productId,
            imageId,
            cancellationToken);

        if (!deleted)
            return NotFound("Image not found");

        return NoContent();
    }

    [HttpPatch("{imageId:int}/primary")]
    public async Task<IActionResult> SetPrimary(
        int productId,
        int imageId,
        CancellationToken cancellationToken)
    {
        var updated = await productImageService.SetPrimaryAsync(
            productId,
            imageId,
            cancellationToken);

        if (!updated)
            return NotFound("Image not found");

        return Ok();
    }

    [HttpPatch("order")]
    public async Task<IActionResult> UpdateOrder(
    int productId,
    [FromBody] ProductImageOrderDTO dto,
    CancellationToken cancellationToken)
    {
        if (dto.ImageIds == null || dto.ImageIds.Count == 0)
            return BadRequest("Image ids are required");

        var updated = await productImageService.UpdateOrderAsync(
            productId,
            dto.ImageIds,
            cancellationToken);

        if (!updated)
            return BadRequest("Invalid image order");

        return Ok();
    }
}
