using Shop.Application.DTOs.UserDTOs;

namespace Shop.Application.Interfaces.Services;

public interface IUserAddressService
{
    Task<UserAddressReadDTO> CreateAsync(Guid userId, UserAddressCreateDTO dto, CancellationToken cancellationToken = default);
    Task<IReadOnlyList<UserAddressReadDTO>> GetAllAsync(Guid userId, CancellationToken cancellationToken = default);
    Task<UserAddressReadDTO?> GetByIdAsync(Guid id, Guid userId, CancellationToken cancellationToken = default);
    Task<bool> DeleteAsync(Guid id, Guid userId, CancellationToken cancellationToken = default);
}
