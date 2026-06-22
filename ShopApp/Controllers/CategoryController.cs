using Microsoft.AspNetCore.Mvc;
using ShopApp.Interfaces;

namespace ShopApp.Controllers;

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
