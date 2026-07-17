using System.Security.Cryptography;
using Shop.Application.Interfaces.Services;
using Shop.Domain.Models;
using Shop.Infrastructure.Configuration;
using Microsoft.Extensions.Options;

namespace Shop.Infrastructure.Services;

public class RefreshTokenService : IRefreshTokenService
{
    private readonly JWTSettings _jwtSettings;

    public RefreshTokenService(IOptions<JWTSettings> jwtOptions)
    {
        _jwtSettings = jwtOptions.Value;
    }

    public RefreshToken GenerateRefreshToken(Guid userId)
    {
        return new RefreshToken
        {
            Token = Convert.ToBase64String(RandomNumberGenerator.GetBytes(64)),
            UserId = userId,
            ExpiresAt = DateTime.UtcNow.AddDays(_jwtSettings.ExpiresRefreshTokenDay),
            IsRevoked = false
        };
    }
}