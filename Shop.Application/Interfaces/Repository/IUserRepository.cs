namespace Shop.Application.Interfaces.Repository;

public interface IUserRepository
{
    Task<List<User>> GetAllAsync(CancellationToken cancellationToken = default);
    Task<User?> GetByIdAsync(Guid id, CancellationToken cancellationToken = default);
    Task<User?> GetByIdForUpdateAsync(Guid id, CancellationToken cancellationToken = default);
    Task<User?> GetByEmailAsync(string email, CancellationToken cancellationToken = default);
    Task<string> GetEmailAsync(Guid userId, CancellationToken cancellationToken = default);
    Task<bool> IsEmailInUseAsync(string email, CancellationToken cancellationToken = default);
    Task<User?> CreateAsync(User user, CancellationToken cancellationToken = default);
    Task<bool> UpdateAsync(User user, CancellationToken cancellationToken = default);
}