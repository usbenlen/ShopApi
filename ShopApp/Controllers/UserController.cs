using Microsoft.AspNetCore.Mvc;
using ShopApp.Filters;
using ShopApp.Interfaces;
using ShopDomain.Models;

namespace ShopApp.Controllers;

[ApiController]
[Route("api/[controller]")] //https://ip:port/api/user
[UserFilter]
public class UserController(IUserService _userService) : ControllerBase
{
    [HttpPost("register")]
    public IActionResult Register([FromBody] User user)
    {
        return Ok(_userService.Register(user));
    }
}
