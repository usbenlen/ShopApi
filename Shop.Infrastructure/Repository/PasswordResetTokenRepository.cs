using Microsoft.EntityFrameworkCore;
using Shop.Application.Interfaces.Repository;
using Shop.Domain.Models;
using Shop.Infrastructure.Data;

namespace Shop.Infrastructure.Repository;

public class PasswordResetTokenRepository(ShopDbContext context) : IPasswordResetTokenRepository
{
    public async Task CreateAsync(PasswordResetToken token, CancellationToken cancellationToken = default)
    {
        await context.PasswordResetTokens.AddAsync(token, cancellationToken);

        await context.SaveChangesAsync(cancellationToken);
    }

    public async Task<PasswordResetToken?> GetActiveByHashAsync(string tokenHash, CancellationToken cancellationToken = default)
    {
        var now = DateTime.UtcNow;

        return await context.PasswordResetTokens
            .Include(x => x.User)
            .FirstOrDefaultAsync(
                x => x.TokenHash == tokenHash && x.UsedAt == null && x.ExpiresAt > now,
                cancellationToken);
    }

    public async Task InvalidateActiveTokensAsync(Guid userId, CancellationToken cancellationToken = default)
    {
        var now = DateTime.UtcNow;

        await context.PasswordResetTokens
            .Where(x =>
                x.UserId == userId &&
                x.UsedAt == null &&
                x.ExpiresAt > now)
            .ExecuteUpdateAsync(
                setters => setters
                    .SetProperty(x => x.UsedAt, now),
                cancellationToken);
    }

    public async Task UpdateAsync(PasswordResetToken token, CancellationToken cancellationToken = default)
    {
        context.PasswordResetTokens.Update(token);

        await context.SaveChangesAsync(cancellationToken);
    }
}