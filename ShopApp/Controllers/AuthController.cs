using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Google;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Shop.Application.DTOs.AuthDTOs;
using Shop.Application.DTOs.UserDTOs;
using Shop.Application.Interfaces.Services;
using System.Security.Claims;

namespace Shop.Api.Controllers;

[ApiController]
[Route("api/[controller]")] //https://ip:port/api/user

public class AuthController(IAuthService _authService, IPasswordService _passwordService, IOAuthCodeService _oauthCodeService, IUserService _userService) : ControllerBase
{
    [HttpPost("register")]
    public async Task<IActionResult> RegisterUser([FromBody] UserCreateDTO dto, CancellationToken cancellationToken)
    {
        var (user, accessToken, refreshToken) = await _authService.RegisterAsync(dto, cancellationToken);
        if (user == null || accessToken == null || refreshToken == null)
            return Conflict(new { message = "This Email is already taken" });

        Response.Cookies.Append(
            "refresh_token",
            refreshToken.Token,
            RefreshTokenCookieOptions(refreshToken.ExpiresAt));


        return Ok(new {user, accessToken});
    }

    [HttpPost("login")]
    public async Task<IActionResult> Login([FromBody] UserLoginDTO dto, CancellationToken cancellationToken)
    {
        var result = await _authService.LoginAsync(dto, cancellationToken);
        if (result == null) return Unauthorized(new { message = "Invalid email or password" });

        var (accessToken, refreshToken) = result.Value;

        Response.Cookies.Append(
            "refresh_token",
            refreshToken.Token,
            RefreshTokenCookieOptions(refreshToken.ExpiresAt));

        return Ok(new { accessToken });
    }

    [HttpPost("refresh")]
    // з Refresh Token Rotation
    public async Task<IActionResult> Refresh(CancellationToken cancellationToken)
    {
        var refreshToken = Request.Cookies["refresh_token"];
        if (string.IsNullOrEmpty(refreshToken)) return Unauthorized(new { message = "Refresh token not found" });

        var result = await _authService.RefreshAsync(refreshToken, cancellationToken);
        if (result == null) return Unauthorized(new { message = "Invalid refresh token" });

        var (accessToken, newRefreshToken) = result.Value;

        Response.Cookies.Append(
            "refresh_token",
            newRefreshToken.Token,
            RefreshTokenCookieOptions(newRefreshToken.ExpiresAt));

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

        DeleteRefreshTokenCookie();

        return Ok(new
        {
            message = "Password changed successfully"
        });
    }

    // Вхід через Google
    [HttpGet("login-google")]
    [AllowAnonymous]
    public IActionResult LoginGoogle()
    {
        var redirectUrl = Url.Action(
            nameof(ExternalResponse),
            "Auth",
            values: null,
            protocol: Request.Scheme);

        var properties = new AuthenticationProperties
        {
            RedirectUri = redirectUrl
        };

        return Challenge(properties, GoogleDefaults.AuthenticationScheme);
    }

    [HttpGet("external-response")]
    [AllowAnonymous]
    public async Task<IActionResult> ExternalResponse(CancellationToken cancellationToken)
    {
        var result = await HttpContext.AuthenticateAsync("External");

        if (!result.Succeeded)
            return BadRequest(new
            {
                message = "Google authentication failed"
            });

        var claims = result.Principal?
            .Identities
            .FirstOrDefault()?
            .Claims
            .ToList();

        if (claims == null)
            return BadRequest(new
            {
                message = "Google claims not found"
            });

        await HttpContext.SignOutAsync("External");

        var email = claims
            .FirstOrDefault(c => c.Type == ClaimTypes.Email)
            ?.Value;

        var googleId = claims
            .FirstOrDefault(c => c.Type == ClaimTypes.NameIdentifier)
            ?.Value;

        var emailVerified = claims
            .FirstOrDefault(c => c.Type == "email_verified")
            ?.Value;

        if (string.IsNullOrWhiteSpace(email) || string.IsNullOrWhiteSpace(googleId))
        {
            return BadRequest(new
            {
                message = "Required Google claims are missing"
            });
        }

        if (!bool.TryParse(emailVerified, out var isEmailVerified) || !isEmailVerified)
        {
            return BadRequest(new
            {
                message = "Google email is not verified"
            });
        }

        var loginResult = await _authService.LoginWithGoogleAsync(googleId, email, cancellationToken);

        if (loginResult == null)
            return Unauthorized(new
            {
                message = "Unable to login with Google"
            });

        var (accessToken, refreshToken) = loginResult.Value;

        Response.Cookies.Append(
            "refresh_token",
            refreshToken.Token,
            RefreshTokenCookieOptions(refreshToken.ExpiresAt));

        var code = await _oauthCodeService.CreateAsync(accessToken, cancellationToken);

        var frontendUrl = "https://localhost:5173/auth/google/callback";

        return Redirect($"{frontendUrl}?code={Uri.EscapeDataString(code)}");
    }

    [HttpPost("google/exchange")]
    [AllowAnonymous]
    public async Task<IActionResult> ExchangeGoogleCode([FromBody] OAuthCodeDTO dto, CancellationToken cancellationToken)
    {
        if (string.IsNullOrWhiteSpace(dto.Code))
            return BadRequest(new
            {
                message = "Code is required"
            });

        var accessToken = await _oauthCodeService.ConsumeAsync(dto.Code, cancellationToken);

        if (string.IsNullOrWhiteSpace(accessToken))
            return Unauthorized(new
            {
                message = "Invalid or expired OAuth code"
            });

        return Ok(new
        {
            accessToken
        });
    }

    [HttpPost("logout")]
    [Authorize]
    public IActionResult Logout()
    {
        DeleteRefreshTokenCookie();

        return Ok(new
        {
            message = "Вихід успішний"
        });
    }


    [HttpGet("me")]
    [Authorize]
    public async Task<IActionResult> Me(CancellationToken cancellationToken)
    {
        var userIdClaim = User.FindFirstValue(ClaimTypes.NameIdentifier);

        if (!Guid.TryParse(userIdClaim, out var userId)) return Unauthorized();

        var user = await _userService.GetByIdAsync(userId, cancellationToken);

        if (user == null)
            return NotFound(new
            {
                message = "User not found"
            });

        var result = new CurrentUserDTO
        {
            Id = user.Id,
            Email = user.Email
        };

        return Ok(result);
    }

    private static CookieOptions RefreshTokenCookieOptions(DateTime expiresAt)
    {
        return new CookieOptions
        {
            HttpOnly = true,
            Secure = true,
            SameSite = SameSiteMode.Lax,
            Path = "/",
            Expires = expiresAt
        };
    }

    private void DeleteRefreshTokenCookie()
    {
        Response.Cookies.Delete(
            "refresh_token",
            new CookieOptions
            {
                HttpOnly = true,
                Secure = true,
                SameSite = SameSiteMode.Lax,
                Path = "/"
            });
    }

}
