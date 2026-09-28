using Microsoft.EntityFrameworkCore;
using Shop.Application.Interfaces.Repository;
using Shop.Domain.Models;
using Shop.Infrastructure.Data;

namespace Shop.Infrastructure.Repository;

public class ProviderRepository(ShopDbContext context) : IProviderRepository
{
    public async Task<Provider?> GetByNameAsync(string name, CancellationToken cancellationToken = default)
    {
        return await context.Providers.FirstOrDefaultAsync(x => x.Name == name, cancellationToken);
    }
}
