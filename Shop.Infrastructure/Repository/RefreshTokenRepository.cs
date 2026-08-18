using Microsoft.EntityFrameworkCore;
using Shop.Application.Interfaces.Repository;
using Shop.Domain.Models;
using Shop.Infrastructure.Data;

namespace Shop.Infrastructure.Repository;

public class RefreshTokenRepository(ShopDbContext context) : IRefreshTokenRepository
{
    public async Task AddAsync(RefreshToken refreshToken, CancellationToken cancellationToken = default)
    {
        await context.RefreshTokens.AddAsync(refreshToken, cancellationToken);

        await context.SaveChangesAsync(cancellationToken);
    }

    public async Task UpdateAsync(RefreshToken refreshToken, CancellationToken cancellationToken = default)
    {
        context.RefreshTokens.Update(refreshToken);

        await context.SaveChangesAsync(cancellationToken);
    }

    public async Task<RefreshToken?> GetByTokenAsync(string token, CancellationToken cancellationToken = default)
    {
        return await context.RefreshTokens
            .Include(x => x.User)
            .FirstOrDefaultAsync(x => x.Token == token, cancellationToken);
    }

    public async Task RevokeAllForUserAsync(Guid userId, CancellationToken cancellationToken = default)
    {
        await context.RefreshTokens
            .Where(x =>
                x.UserId == userId &&
                !x.IsRevoked)
            .ExecuteUpdateAsync(
                setters => setters.SetProperty(x => x.IsRevoked, true), 
                cancellationToken);
    }
}