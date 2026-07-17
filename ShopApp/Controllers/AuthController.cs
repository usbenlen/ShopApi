using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.AspNetCore.Mvc;
using Shop.Application.DTOs.UserDTOs;
using Shop.Application.Interfaces.Services;

namespace Shop.Api.Controllers;

[ApiController]
[Route("api/[controller]")] //https://ip:port/api/user

public class AuthController(IAuthService _authService) : ControllerBase
{
    [HttpPost("register")]
    public async Task<IActionResult> RegisterUser([FromBody] UserCreateDTO dto)
    {
        var (user, accessToken, refreshToken) = await _authService.RegisterAsync(dto);
        if (user == null) return Conflict(new { message = "This Email is already taken" });

        Response.Cookies.Append(
        "refresh_token",
        refreshToken.Token,
        new CookieOptions
        {
            HttpOnly = true,
            Secure = true,
            SameSite = SameSiteMode.Strict,
            Expires = refreshToken.ExpiresAt
        });

        return Ok(new {user, accessToken});
    }

    [HttpPost("login")]
    public async Task<IActionResult> Login([FromBody] UserLoginDTO dto)
    {
        var result = await _authService.LoginAsync(dto);
        if (result == null) return Unauthorized(new { message = "Invalid email or password" });

        var (accessToken, refreshToken) = result.Value;

        Response.Cookies.Append(
            "refresh_token",
            refreshToken.Token,
            new CookieOptions
            {
                HttpOnly = true,
                Secure = true,
                SameSite = SameSiteMode.Strict,
                Expires = refreshToken.ExpiresAt
            });

        return Ok(new { accessToken });
    }

    [HttpPost("refresh")]
    // з Refresh Token Rotation
    public async Task<IActionResult> Refresh()
    {
        var refreshToken = Request.Cookies["refresh_token"];
        if (string.IsNullOrEmpty(refreshToken)) return Unauthorized(new { message = "Refresh token not found" });

        var result = await _authService.RefreshAsync(refreshToken);
        if (result == null) return Unauthorized(new { message = "Invalid refresh token" });

        var (accessToken, newRefreshToken) = result.Value;

        Response.Cookies.Append(
        "refresh_token",
        newRefreshToken.Token,
        new CookieOptions
        {
            HttpOnly = true,
            Secure = true,
            SameSite = SameSiteMode.Strict,
            Expires = newRefreshToken.ExpiresAt
        });

        return Ok(new { accessToken });
    }
}
