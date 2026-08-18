using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Shop.Application.DTOs.AuthDTOs;
using Shop.Application.DTOs.UserDTOs;
using Shop.Application.Interfaces.Services;
using System.Security.Claims;

namespace Shop.Api.Controllers;

[ApiController]
[Route("api/[controller]")] //https://ip:port/api/user

public class AuthController(IAuthService _authService, IPasswordService _passwordService) : ControllerBase
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

    [HttpPost("forgot-password")]
    [AllowAnonymous]
    public async Task<IActionResult> ForgotPassword([FromBody] ForgotPasswordDTO dto, CancellationToken cancellationToken)
    {
        await _passwordService.RequestPasswordResetAsync(dto.Email, cancellationToken);

        return Ok(new
        {
            message = "If the email exists, a password reset link has been sent"
        });
    }

    [HttpPost("reset-password")]
    [AllowAnonymous]
    public async Task<IActionResult> ResetPassword([FromBody] ResetPasswordDTO dto, CancellationToken cancellationToken)
    {
        var result = await _passwordService.ResetPasswordAsync(dto, cancellationToken);

        if (!result)
            return BadRequest(new
            {
                message = "Invalid or expired password reset token"
            });

        return Ok(new
        {
            message = "Password has been reset successfully"
        });
    }

    [HttpPost("change-password")]
    [Authorize]
    public async Task<IActionResult> ChangePassword([FromBody] ChangePasswordDTO dto, CancellationToken cancellationToken)
    {
        var userIdClaim = User.FindFirstValue(ClaimTypes.NameIdentifier);

        if (!Guid.TryParse(userIdClaim, out var userId)) return Unauthorized();

        var result = await _passwordService.ChangePasswordAsync(userId, dto, cancellationToken);

        if (!result)
            return BadRequest(new
            {
                message ="Current password is invalid or the new password is invalid"
            });

        Response.Cookies.Delete("refresh_token");

        return Ok(new
        {
            message = "Password changed successfully"
        });
    }
}
