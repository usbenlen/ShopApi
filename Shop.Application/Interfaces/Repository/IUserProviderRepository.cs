using Shop.Domain.Models;

namespace Shop.Application.Interfaces.Repository;

public interface IUserProviderRepository
{
    Task<UserProvider?> GetAsync(int providerId, string numberProvider, CancellationToken cancellationToken = default);

    Task<UserProvider?> GetByUserAsync(Guid userId, int providerId, CancellationToken cancellationToken = default);

    Task<UserProvider> AddAsync(UserProvider userProvider, CancellationToken cancellationToken = default);

    Task<bool> ExistsAsync(Guid userId, int providerId, CancellationToken cancellationToken = default);
}
