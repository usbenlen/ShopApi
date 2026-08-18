using Shop.Application.DTOs.UserDTOs;

namespace Shop.Application.Interfaces.Services;

public interface IUserService
{
    Task<UserReadDTO?> CreateStaffAsync(CreateStaffUserDTO dto);
    Task<IReadOnlyList<UserReadDTO>> GetAllAsync();
    Task<UserReadDTO?> GetByIdAsync(Guid id);
    Task<bool> UpdateRoleAsync(Guid id, UpdateUserRoleDTO dto);
    Task<bool> UpdateStatusAsync(Guid id, UpdateUserStatusDTO dto);
}