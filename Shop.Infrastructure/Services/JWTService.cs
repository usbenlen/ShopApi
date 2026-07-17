using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Tokens;
using Shop.Application.DTOs.UserDTOs;
using Shop.Application.Interfaces.Services;
using Shop.Infrastructure.Configuration;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;

namespace Shop.Infrastructure.Services;

public class JWTService : IJWTService
{
    private readonly JWTSettings _jwtSettings;
    private readonly SigningCredentials _credentials;
    private readonly JwtSecurityTokenHandler _tokenHandler = new();

    public JWTService(IOptions<JWTSettings> jwtOptions)
    {
        _jwtSettings = jwtOptions.Value;

        var key = Encoding.UTF8.GetBytes(_jwtSettings.Key);
        var signingKey = new SymmetricSecurityKey(key);

        _credentials = new SigningCredentials(signingKey, SecurityAlgorithms.HmacSha256);
    }

    public string GenerateAccessToken(UserTokenDTO user)
    {
        var claims = new List<Claim>
        {
            new Claim(ClaimTypes.NameIdentifier, user.Id.ToString()),
            new Claim(ClaimTypes.Email, user.Email),
            new Claim(ClaimTypes.Role, user.Role.ToString()),
            new Claim(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString())
        };

        var token = new JwtSecurityToken(
            issuer: _jwtSettings.Issuer,
            audience: _jwtSettings.Audience,
            claims: claims,
            expires: DateTime.UtcNow.AddMinutes(_jwtSettings.ExpiresMinutes),
            signingCredentials: _credentials);

        return _tokenHandler.WriteToken(token);
    }
}