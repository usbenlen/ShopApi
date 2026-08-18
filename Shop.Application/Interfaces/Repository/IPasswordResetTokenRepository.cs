using Shop.Domain.Models;

namespace Shop.Application.Interfaces.Repository;

public interface IPasswordResetTokenRepository
{
    Task CreateAsync(PasswordResetToken token, CancellationToken cancellationToken = default);
    Task<PasswordResetToken?> GetActiveByHashAsync(string tokenHash, CancellationToken cancellationToken = default);
    Task InvalidateActiveTokensAsync(Guid userId, CancellationToken cancellationToken = default);
    Task UpdateAsync(PasswordResetToken token, CancellationToken cancellationToken = default);
}