using Microsoft.EntityFrameworkCore;
using Shop.Application.Interfaces.Repository;
using Shop.Infrastructure.Data;

namespace Shop.Infrastructure.Repository;

public class AuthRepository(ShopDbContext context) : IAuthRepository
{
    public async Task<bool> IsEmailInUseAsync(string email)
    {
        return await context.Users.AnyAsync(x => x.Email == email);
    }

    public async Task<User?> RegisterUserAsync(User user)
    {
        await context.Users.AddAsync(user);
        await context.SaveChangesAsync();

        return user;
    }

    public async Task<User?> GetByEmailAsync(string email)
    {
        return await context.Users.FirstOrDefaultAsync(x => x.Email == email);
    }
}