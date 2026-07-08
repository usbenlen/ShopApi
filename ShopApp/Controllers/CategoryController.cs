using Microsoft.AspNetCore.Mvc;
using Shop.Application.Interfaces.Services;
using Shop.Application.DTOs.CategoryDTOs;
using Shop.Api.Requests.Categories;
using Shop.Api.Interfaces;

namespace Shop.Api.Controllers;

[ApiController]
[Route("api/[controller]")] //https://ip:port/api/category
public class CategoryController(ICategoryService _categoryService, IImageService _imageService, IConfiguration _configuration) : ControllerBase
{
    [HttpPost]
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

        int? id = await _categoryService.CreateCategoryAsync(createDTO);
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

    [HttpGet("{id}")]
    public async Task<IActionResult> GetCategoryById(int id)
    {
        var category = await _categoryService.GetCategoryByIdAsync(id);
        if (category == null) return NotFound();

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
    public async Task<IActionResult> UpdateCategory(int id, [FromBody] CategoryUpdateDTO dto)
    {
        bool updated = await _categoryService.UpdateCategoryAsync(id, dto);
        if (!updated) return NotFound();

        return Ok("Category updated");
    }
}
