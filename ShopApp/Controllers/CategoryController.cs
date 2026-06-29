using Microsoft.AspNetCore.Mvc;
using Shop.Api.Interfaces;

namespace Shop.Api.Controllers;

[ApiController]
[Route("api/[controller]")] //https://ip:port/api/category
public class CategoryController(ICategoryService _categoryService) : ControllerBase
{
    [HttpGet]
    public IActionResult GetCategories()
    {
        return Ok(_categoryService.GetAllCategories());
    }
}
