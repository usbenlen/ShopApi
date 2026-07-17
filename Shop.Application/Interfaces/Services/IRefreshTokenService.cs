using Shop.Domain.Models;

namespace Shop.Application.Interfaces.Services;

public interface IRefreshTokenService
{
    RefreshToken GenerateRefreshToken(Guid userId);
}