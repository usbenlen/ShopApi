
namespace Shop.Application.Interfaces.Repository;

public interface IAuthRepository
{
    Task<User?> RegisterUserAsync(User user);
    Task<bool> IsEmailInUseAsync(string email);
    Task<User?> GetByEmailAsync(string email);
}
