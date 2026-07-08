using Microsoft.AspNetCore.Mvc;
using Shop.Api.Filters;
using Shop.Api.Interfaces;
using Shop.Domain.Models;

namespace Shop.Api.Controllers;

[ApiController]
[Route("api/[controller]")] //https://ip:port/api/user
//[UserFilter]
public class UserController(IUserService _userService) : ControllerBase
{
    [HttpPost("register")]
    public IActionResult Register([FromBody] User user)
    {
        return Ok(_userService.Register(user));
    }
}
