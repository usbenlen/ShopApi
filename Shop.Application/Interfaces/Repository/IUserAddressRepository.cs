using Shop.Domain.Models;

namespace Shop.Application.Interfaces.Repository;

public interface IUserAddressRepository
{
    Task<UserAddress?> GetByIdAsync(Guid id, Guid userId, CancellationToken cancellationToken = default);
    Task<List<UserAddress>> GetByUserIdAsync(Guid userId, CancellationToken cancellationToken = default);
    Task<UserAddress> CreateAsync(UserAddress address, CancellationToken cancellationToken = default);
    Task<bool> DeleteAsync(Guid id, Guid userId, CancellationToken cancellationToken = default);
}
