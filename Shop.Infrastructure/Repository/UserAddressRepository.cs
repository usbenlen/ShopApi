using Microsoft.EntityFrameworkCore;
using Shop.Application.Interfaces.Repository;
using Shop.Domain.Models;
using Shop.Infrastructure.Data;

namespace Shop.Infrastructure.Repository;

public class UserAddressRepository(ShopDbContext context) : IUserAddressRepository
{
    public async Task<UserAddress?> GetByIdAsync(Guid id, Guid userId, CancellationToken cancellationToken = default)
    {
        return await context.UserAddresses
            .AsNoTracking()
            .FirstOrDefaultAsync(x => x.Id == id && x.UserId == userId, cancellationToken);
    }

    public async Task<List<UserAddress>> GetByUserIdAsync(Guid userId, CancellationToken cancellationToken = default)
    {
        return await context.UserAddresses
            .AsNoTracking()
            .Where(x => x.UserId == userId)
            .OrderByDescending(x => x.CreatedAt)
            .ToListAsync(cancellationToken);
    }

    public async Task<UserAddress> CreateAsync(UserAddress address, CancellationToken cancellationToken = default)
    {
        await context.UserAddresses.AddAsync(address, cancellationToken);
        await context.SaveChangesAsync(cancellationToken);

        return address;
    }

    public async Task<bool> DeleteAsync(Guid id, Guid userId, CancellationToken cancellationToken = default)
    {
        var address = await context.UserAddresses
            .FirstOrDefaultAsync(x => x.Id == id && x.UserId == userId, cancellationToken);

        if (address == null) return false;

        context.UserAddresses.Remove(address);

        var affectedRows = await context.SaveChangesAsync(cancellationToken);

        return affectedRows > 0;
    }
}
