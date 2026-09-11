using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Shop.Application.DTOs.UserDTOs;
using Shop.Application.Interfaces.Services;

namespace Shop.Api.Controllers;

[ApiController]
[Route("api/[controller]")]
[Authorize(Roles = "Admin")]
public class UserController(IUserService _userService) : ControllerBase
{
    /// <summary>
    /// Створити нового Admin або Moderator
    /// </summary>
    [HttpPost("staff")]
    public async Task<IActionResult> CreateStaff([FromBody] CreateStaffUserDTO dto, CancellationToken cancellationToken)
    {
        var user = await _userService.CreateStaffAsync(dto, cancellationToken);

        if (user == null)
            return Conflict(new
            {
                message = "User already exists or invalid role"
            });

        return Created($"/api/User/{user.Id}", user);
    }

    /// <summary>
    /// Отримати всіх користувачів
    /// </summary>
    [HttpGet]
    public async Task<IActionResult> GetAll(CancellationToken cancellationToken)
    {
        var users = await _userService.GetAllAsync(cancellationToken);

        return Ok(users);
    }

    /// <summary>
    /// Отримати користувача
    /// </summary>
    [HttpGet("{id:guid}")]
    public async Task<IActionResult> GetById(Guid id, CancellationToken cancellationToken)
    {
        var user = await _userService.GetByIdAsync(id, cancellationToken);

        if (user == null) return NotFound();

        return Ok(user);
    }

    /// <summary>
    /// Змінити роль користувача
    /// </summary>
    [HttpPut("{id:guid}/role")]
    public async Task<IActionResult> UpdateRole(Guid id, [FromBody] UpdateUserRoleDTO dto, CancellationToken cancellationToken)
    {
        var result = await _userService.UpdateRoleAsync(id, dto, cancellationToken);

        if (!result) return NotFound();

        return Ok(new
        {
            message = "User role successfully updated"
        });
    }

    /// <summary>
    /// Активувати або деактивувати користувача
    /// </summary>
    [HttpPut("{id:guid}/status")]
    public async Task<IActionResult> UpdateStatus(Guid id, [FromBody] UpdateUserStatusDTO dto, CancellationToken cancellationToken)
    {
        var result = await _userService.UpdateStatusAsync(id, dto, cancellationToken);

        if (!result) return NotFound();

        return Ok(new
        {
            message = "User status successfully updated"
        });
    }
}