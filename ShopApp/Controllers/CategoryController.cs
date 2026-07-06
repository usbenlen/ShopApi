using Microsoft.AspNetCore.Mvc;
using Shop.Application.Interfaces.Services;
using Shop.Application.DTOs.CategoryDTOs;

namespace Shop.Api.Controllers;

[ApiController]
[Route("api/[controller]")] //https://ip:port/api/category
public class CategoryController(ICategoryService _categoryService) : ControllerBase
{
    [HttpPost]
    public async Task<IActionResult> CreateCategory([FromBody] CategoryCreateDTO dto)
    {
        int? id = await _categoryService.CreateCategoryAsync(dto);
        return Ok($"Category created {id}"); // 200 status
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
}
