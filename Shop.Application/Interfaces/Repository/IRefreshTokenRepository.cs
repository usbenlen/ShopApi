using Shop.Domain.Models;

namespace Shop.Application.Interfaces.Repository;

public interface IRefreshTokenRepository
{
    Task AddAsync(RefreshToken refreshToken);
    Task UpdateAsync(RefreshToken refreshToken);
    Task<RefreshToken?> GetByTokenAsync(string token);
}