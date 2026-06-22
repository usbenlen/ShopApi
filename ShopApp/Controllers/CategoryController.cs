using Microsoft.AspNetCore.Mvc;

namespace ShopApp.Controllers;

[ApiController]
[Route("api/[controller]")] //https://ip:port/api/category
public class CategoryController : ControllerBase
{
    [HttpGet]
    public IActionResult Test()
    {
        return Ok("Method Test"); // Status 200
    }
}
