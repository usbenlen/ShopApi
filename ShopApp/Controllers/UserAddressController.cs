using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Shop.Application.DTOs.UserDTOs;
using Shop.Application.Interfaces.Services;
using System.Security.Claims;

namespace Shop.Api.Controllers;

[ApiController]
[Route("api/[controller]")]
[Authorize]
public class UserAddressController(IUserAddressService _userAddressService) : ControllerBase
{
    /// <summary>
    /// Додати адресу доставки поточного користувача
    /// </summary>
    [HttpPost]
    public async Task<IActionResult> Create([FromBody] UserAddressCreateDTO dto, CancellationToken cancellationToken)
    {
        var userIdClaim = User.FindFirst(ClaimTypes.NameIdentifier);

        if (userIdClaim == null || !Guid.TryParse(userIdClaim.Value, out Guid userId))
            return Unauthorized();

        var address = await _userAddressService.CreateAsync(userId, dto, cancellationToken);

        return CreatedAtAction(nameof(GetById), new { id = address.Id }, address);
    }

    /// <summary>
    /// Отримати всі адреси поточного користувача
    /// </summary>
    [HttpGet]
    public async Task<IActionResult> GetAll(CancellationToken cancellationToken)
    {
        var userIdClaim = User.FindFirst(ClaimTypes.NameIdentifier);

        if (userIdClaim == null || !Guid.TryParse(userIdClaim.Value, out Guid userId))
            return Unauthorized();

        var addresses = await _userAddressService.GetAllAsync(userId, cancellationToken);

        return Ok(addresses);
    }

    /// <summary>
    /// Отримати адресу поточного користувача за ID
    /// </summary>
    [HttpGet("{id:guid}")]
    public async Task<IActionResult> GetById(Guid id, CancellationToken cancellationToken)
    {
        var userIdClaim = User.FindFirst(ClaimTypes.NameIdentifier);

        if (userIdClaim == null || !Guid.TryParse(userIdClaim.Value, out Guid userId))
            return Unauthorized();

        var address = await _userAddressService.GetByIdAsync(id, userId, cancellationToken);
        if (address == null) return NotFound();

        return Ok(address);
    }


    /// <summary>
    /// Видалити адресу поточного користувача
    /// </summary>
    [HttpDelete("{id:guid}")]
    public async Task<IActionResult> Delete(Guid id, CancellationToken cancellationToken)
    {
        var userIdClaim = User.FindFirst(ClaimTypes.NameIdentifier);

        if (userIdClaim == null || !Guid.TryParse(userIdClaim.Value, out Guid userId))
            return Unauthorized();

        var deleted = await _userAddressService.DeleteAsync(id, userId, cancellationToken);
        if (!deleted) return NotFound();

        return NoContent();
    }
}
