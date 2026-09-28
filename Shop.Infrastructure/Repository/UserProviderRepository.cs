using Microsoft.EntityFrameworkCore;
using Shop.Application.Interfaces.Repository;
using Shop.Domain.Models;
using Shop.Infrastructure.Data;

namespace Shop.Infrastructure.Repository;

public class UserProviderRepository(ShopDbContext context) : IUserProviderRepository
{
    public async Task<UserProvider?> GetAsync(int providerId, string numberProvider, CancellationToken cancellationToken = default)
    {
        return await context.UserProviders
            .Include(x => x.User)
            .Include(x => x.Provider)
            .FirstOrDefaultAsync(
                x =>
                    x.ProviderId == providerId &&
                    x.NumberProvider == numberProvider,
                cancellationToken);
    }

    public async Task<UserProvider?> GetByUserAsync(Guid userId, int providerId, CancellationToken cancellationToken = default)
    {
        return await context.UserProviders
            .FirstOrDefaultAsync(
                x =>
                    x.UserId == userId &&
                    x.ProviderId == providerId,
                cancellationToken);
    }

    public async Task<UserProvider> AddAsync(UserProvider userProvider, CancellationToken cancellationToken = default)
    {
        await context.UserProviders.AddAsync(userProvider, cancellationToken);

        await context.SaveChangesAsync(cancellationToken);

        return userProvider;
    }

    public async Task<bool> ExistsAsync(Guid userId, int providerId, CancellationToken cancellationToken = default)
    {
        return await context.UserProviders.AnyAsync(
            x =>
                x.UserId == userId &&
                x.ProviderId == providerId,
            cancellationToken);
    }
}
