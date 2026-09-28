using Shop.Domain.Models;

namespace Shop.Application.Interfaces.Repository;

public interface IProviderRepository
{
    Task<Provider?> GetByNameAsync(string name, CancellationToken cancellationToken = default);
}