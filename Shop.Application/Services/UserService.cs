using AutoMapper;
using Shop.Application.DTOs.UserDTOs;
using Shop.Application.Interfaces.Helpers;
using Shop.Application.Interfaces.Repository;
using Shop.Application.Interfaces.Services;
using Shop.Domain.Enums;
using System.Security.Cryptography;

namespace Shop.Application.Services;

public class UserService(
    IUserRepository _repository,
    IHashHelper _hashHelper,
    IPasswordService _passwordService,
    IMapper _mapper) : IUserService
{
    public async Task<UserReadDTO?> CreateStaffAsync(CreateStaffUserDTO dto, CancellationToken cancellationToken = default)
    {
        if (dto.Role != UserRole.Admin && dto.Role != UserRole.Moderator)
            return null;

        var email = dto.Email.Trim().ToLowerInvariant();

        if (await _repository.IsEmailInUseAsync(email, cancellationToken)) return null;

        var user = new User
        {
            Email = email,
            PasswordHash = _hashHelper.Hash(Convert.ToBase64String(RandomNumberGenerator.GetBytes(48))),
            Role = dto.Role,
            IsActive = true
        };

        await _repository.CreateAsync(user, cancellationToken);

        await _passwordService.SendPasswordSetupAsync(user, cancellationToken);

        return _mapper.Map<UserReadDTO>(user);
    }

    public async Task<IReadOnlyList<UserReadDTO>> GetAllAsync(CancellationToken cancellationToken = default)
    {
        var users = await _repository.GetAllAsync(cancellationToken);

        return _mapper.Map<List<UserReadDTO>>(users);
    }

    public async Task<UserReadDTO?> GetByIdAsync(Guid id, CancellationToken cancellationToken = default)
    {
        var user = await _repository.GetByIdAsync(id, cancellationToken);

        if (user == null) return null;

        return _mapper.Map<UserReadDTO>(user);
    }

    public async Task<bool> UpdateRoleAsync(Guid id, UpdateUserRoleDTO dto, CancellationToken cancellationToken = default)
    {
        var user = await _repository.GetByIdForUpdateAsync(id, cancellationToken);

        if (user == null) return false;

        if (dto.Role != UserRole.User && dto.Role != UserRole.Admin && dto.Role != UserRole.Moderator)
            return false;

        user.Role = dto.Role;

        return await _repository.UpdateAsync(user, cancellationToken);
    }

    public async Task<bool> UpdateStatusAsync(Guid id, UpdateUserStatusDTO dto, CancellationToken cancellationToken = default)
    {
        var user = await _repository.GetByIdForUpdateAsync(id, cancellationToken);
        if (user == null) return false;

        user.IsActive = dto.IsActive;

        return await _repository.UpdateAsync(user, cancellationToken);
    }
}