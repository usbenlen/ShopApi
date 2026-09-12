using Microsoft.AspNetCore.Mvc;

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
    public async Task<IActionResult> AddImage(
        int productId,
        IFormFile file,
        CancellationToken cancellationToken)
    {
        if (file == null || file.Length == 0)
            return BadRequest("Image is required");

        if (!file.ContentType.StartsWith("image/"))
            return BadRequest("Only image files are allowed");

        try
        {
            var image = await productImageService.AddAsync(
                productId,
                file,
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
}
