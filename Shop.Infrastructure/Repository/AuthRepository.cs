using Shop.Application.Interfaces.Repository;
using Shop.Infrastructure.Data;
using Microsoft.EntityFrameworkCore;

namespace Shop.Infrastructure.Repository;

public class AuthRepository(ShopDbContext _context) : IAuthRepository
{
    public async Task<bool> IsEmailInUseAsync(string email)
    {
        return await _context.Users.AnyAsync(user => user.Email == email);
    }
    
    public async Task<User?> RegisterUserAsync(User user)
    {
        await _context.Users.AddAsync(user);
        await _context.SaveChangesAsync();

        return user;
    }

    public async Task<User?> GetByEmailAsync(string email)
    {
        return await _context.Users.FirstOrDefaultAsync(x => x.Email == email);
    }
}
