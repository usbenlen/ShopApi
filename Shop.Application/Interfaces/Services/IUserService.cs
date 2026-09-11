using Shop.Application.DTOs.UserDTOs;

namespace Shop.Application.Interfaces.Services;

public interface IUserService
{
    Task<UserReadDTO?> CreateStaffAsync(CreateStaffUserDTO dto, CancellationToken cancellationToken = default);
    Task<IReadOnlyList<UserReadDTO>> GetAllAsync(CancellationToken cancellationToken = default);
    Task<UserReadDTO?> GetByIdAsync(Guid id, CancellationToken cancellationToken = default);
    Task<bool> UpdateRoleAsync(Guid id, UpdateUserRoleDTO dto, CancellationToken cancellationToken = default);
    Task<bool> UpdateStatusAsync(Guid id, UpdateUserStatusDTO dto, CancellationToken cancellationToken = default);
}