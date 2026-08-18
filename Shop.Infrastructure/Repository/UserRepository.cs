using Microsoft.EntityFrameworkCore;
using Shop.Application.Interfaces.Repository;
using Shop.Infrastructure.Data;

namespace Shop.Infrastructure.Repository;

public class UserRepository(ShopDbContext context) : IUserRepository
{
    public async Task<List<User>> GetAllAsync(
    CancellationToken cancellationToken = default)
    {
        return await context.Users.AsNoTracking().ToListAsync(cancellationToken);
    }

    public async Task<User?> GetByIdAsync(Guid id, CancellationToken cancellationToken = default)
    {
        return await context.Users.FirstOrDefaultAsync(x => x.Id == id, cancellationToken);
    }

    public async Task<User?> GetByIdForUpdateAsync(Guid id, CancellationToken cancellationToken = default)
    {
        return await context.Users.FirstOrDefaultAsync(user => user.Id == id, cancellationToken);
    }

    public async Task<User?> GetByEmailAsync(string email, CancellationToken cancellationToken = default)
    {
        return await context.Users.FirstOrDefaultAsync(x => x.Email == email, cancellationToken);
    }

    public async Task<bool> IsEmailInUseAsync(string email, CancellationToken cancellationToken = default)
    {
        return await context.Users.AnyAsync(x => x.Email == email, cancellationToken);
    }

    public async Task<User?> CreateAsync(User user, CancellationToken cancellationToken = default)
    {
        await context.Users.AddAsync(user, cancellationToken);

        await context.SaveChangesAsync(cancellationToken);

        return user;
    }

    public async Task<bool> UpdateAsync(User user, CancellationToken cancellationToken = default)
    {
        context.Users.Update(user);

        var affectedRows = await context.SaveChangesAsync(cancellationToken);

        return affectedRows > 0;
    }
}