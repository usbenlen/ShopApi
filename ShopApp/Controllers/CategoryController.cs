using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Shop.Api.Interfaces;
using Shop.Api.Requests.Categories;
using Shop.Application.Commands.Categories.CreateCategory;
using Shop.Application.DTOs.CategoryDTOs;
using Shop.Application.Interfaces.Services;
using Shop.Application.Queries.Category.GetCategoryById;
using Shop.Application.Queries.Category.GetCategoryBySlug;

namespace Shop.Api.Controllers;

[ApiController]
[Route("api/[controller]")] //https://ip:port/api/category
public class CategoryController(ICategoryService _categoryService, IImageService _imageService, IConfiguration _configuration, IMediator _mediator) : ControllerBase
{
    [HttpPost]
    [Authorize]
    public async Task<IActionResult> CreateCategory([FromForm] CategoryCreateRequest dto)
    {
        if (dto.Image != null) 
            dto.ImageURL = (await _imageService.SaveFileAsync(dto.Image, _configuration["DirnameForFiles:Categories"])) ?? string.Empty;

        var createDTO = new CategoryCreateDTO
        {
            Name = dto.Name,
            Slug = dto.Slug,
            Description = dto.Description,
            ImageURL = dto.ImageURL,
            ParentId = dto.ParentId,
        };

        int? id = await _mediator.Send(new CreateCategoryCommand(createDTO));
        //int? id = await _categoryService.CreateCategoryAsync(createDTO);
        return Ok($"Category created {id}"); // 200 status
        //return CreatedAtAction(
        //    nameof(GetCategoryById), // назва методу
        //    new { id }, // параметри маршруту
        //    new { id }); // тіло відповіді
    }

    [HttpGet]
    public async Task<IActionResult> GetCategories()
    {
        var categories = await _categoryService.GetCategoriesAsync();

        return Ok(categories);
    }

    [HttpGet("id/{id}")]
    public async Task<IActionResult> GetCategoryById(int id)
    {
        var category = await _mediator.Send(new GetCategoryByIdQuery(id));
        //var category = await _categoryService.GetCategoryByIdAsync(id);
        if (category == null) return NotFound();

        return Ok(category);
    }

    [HttpGet("{slug}")]
    public async Task<ActionResult<CategoryReadDTO>> GetCategoryBySlug(string slug)
    {
        var category = await _mediator.Send(new GetCategoryBySlugQuery(slug));
        if (category is null) return NotFound();

        return Ok(category);
    }


    [HttpDelete("{id}")]
    public async Task<IActionResult> DeleteCategory(int id)
    {
        bool deleted = await _categoryService.DeleteCategoryAsync(id);
        if (!deleted) return NotFound();

        return NoContent();
    }

    [HttpPut("{id}")]
    public async Task<IActionResult> UpdateCategory(int id, [FromForm] CategoryUpdateRequest dto)
    {
        if (dto.Image != null)
        {
            string? url = await _imageService.SaveFileAsync(dto.Image, _configuration["DirnameForFiles:Categories"]);
            dto.ImageURL = url;
        }

        bool updated = await _categoryService.UpdateCategoryAsync(id, dto);
        if (!updated) return NotFound("Category not found");

        return Ok("Category updated");
    }

    [HttpGet("{id}/parents")]
    public async Task<IActionResult> GetParents(int id)
    {
        var categories = await _categoryService.GetParentCategoriesAsync(id);

        return Ok(categories);
    }

    [HttpGet("{id}/children")]
    public async Task<IActionResult> GetChildren(int id)
    {
        var categories = await _categoryService.GetChildCategoriesAsync(id);

        return Ok(categories);
    }

    [HttpGet("{id}/tree")]
    public async Task<IActionResult> GetTree(int id)
    {
        var tree = await _categoryService.GetCategoryTreeAsync(id);
        if (tree == null) return NotFound();

        return Ok(tree);
    }
}
