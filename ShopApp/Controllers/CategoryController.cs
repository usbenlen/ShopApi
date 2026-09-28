using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Shop.Api.Requests.Categories;
using Shop.Application.Commands.Categories.CreateCategory;
using Shop.Application.DTOs.CategoryDTOs;
using Shop.Application.Interfaces.Services;
using Shop.Application.Queries.Category.GetCategoryById;
using Shop.Application.Queries.Category.GetCategoryBySlug;
using FluentValidation;

namespace Shop.Api.Controllers;

[ApiController]
[Route("api/[controller]")] //https://ip:port/api/category
public class CategoryController(ICategoryService _categoryService, IImageService _imageService, IConfiguration _configuration, IMediator _mediator, IValidator<CategoryCreateDTO> _validator) : ControllerBase
{
    [HttpPost]
    [Authorize]
    public async Task<IActionResult> CreateCategory([FromForm] CategoryCreateRequest dto, CancellationToken cancellationToken)
    {
        if (dto.Image != null)
        {
            await using var stream = dto.Image.OpenReadStream();

            dto.ImageURL =
                await _imageService.SaveFileAsync(
                    stream,
                    dto.Image.FileName,
                    dto.Image.ContentType,
                    _configuration["DirnameForFiles:Categories"] ?? "Categories",
                    cancellationToken)
                ?? string.Empty;
        }

        var createDTO = new CategoryCreateDTO
        {
            Name = dto.Name,
            Slug = dto.Slug,
            Description = dto.Description,
            ImageURL = dto.ImageURL,
            ParentId = dto.ParentId,
        };

        // Валідація DTO
        var result = await _validator.ValidateAsync(createDTO, cancellationToken);
        if (!result.IsValid) return BadRequest(result.Errors);

        int? id = await _mediator.Send(new CreateCategoryCommand(createDTO), cancellationToken);
        //int? id = await _categoryService.CreateCategoryAsync(createDTO);
        return Ok($"Category created {id}"); // 200 status
        //return CreatedAtAction(
        //    nameof(GetCategoryById), // назва методу
        //    new { id }, // параметри маршруту
        //    new { id }); // тіло відповіді
    }

    [HttpGet]
    public async Task<IActionResult> GetCategories(CancellationToken cancellationToken)
    {
        var categories = await _categoryService.GetCategoriesAsync(cancellationToken);

        return Ok(categories);
    }

    [HttpGet("id/{id}")]
    public async Task<IActionResult> GetCategoryById(int id, CancellationToken cancellationToken)
    {
        var category = await _mediator.Send(new GetCategoryByIdQuery(id), cancellationToken);
        //var category = await _categoryService.GetCategoryByIdAsync(id);
        if (category == null) return NotFound();

        return Ok(category);
    }

    [HttpGet("{slug}")]
    public async Task<ActionResult<CategoryReadDTO>> GetCategoryBySlug(string slug, CancellationToken cancellationToken)
    {
        var category = await _mediator.Send(new GetCategoryBySlugQuery(slug), cancellationToken);
        if (category is null) return NotFound();

        return Ok(category);
    }


    [HttpDelete("{id}")]
    public async Task<IActionResult> DeleteCategory(int id, CancellationToken cancellationToken)
    {
        bool deleted = await _categoryService.DeleteCategoryAsync(id, cancellationToken);
        if (!deleted) return NotFound();

        return NoContent();
    }

    [HttpPut("{id}")]
    public async Task<IActionResult> UpdateCategory(int id, [FromForm] CategoryUpdateRequest dto, CancellationToken cancellationToken)
    {
        if (dto.Image != null)
        {
            await using var stream = dto.Image.OpenReadStream();

            dto.ImageURL =
                await _imageService.SaveFileAsync(
                    stream,
                    dto.Image.FileName,
                    dto.Image.ContentType,
                    _configuration["DirnameForFiles:Categories"] ?? "Categories",
                    cancellationToken);
        }

        bool updated = await _categoryService.UpdateCategoryAsync(id, dto, cancellationToken);
        if (!updated) return NotFound("Category not found");

        return Ok("Category updated");
    }

    [HttpGet("{id}/parents")]
    public async Task<IActionResult> GetParents(int id, CancellationToken cancellationToken)
    {
        var categories = await _categoryService.GetParentCategoriesAsync(id, cancellationToken);

        return Ok(categories);
    }

    [HttpGet("{id}/children")]
    public async Task<IActionResult> GetChildren(int id, CancellationToken cancellationToken)
    {
        var categories = await _categoryService.GetChildCategoriesAsync(id, cancellationToken);

        return Ok(categories);
    }

    [HttpGet("{id}/tree")]
    public async Task<IActionResult> GetTree(int id, CancellationToken cancellationToken)
    {
        var tree = await _categoryService.GetCategoryTreeAsync(id, cancellationToken);
        if (tree == null) return NotFound();

        return Ok(tree);
    }
}
