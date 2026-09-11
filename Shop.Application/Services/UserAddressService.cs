using AutoMapper;
using Shop.Application.DTOs.UserDTOs;
using Shop.Application.Interfaces.Repository;
using Shop.Application.Interfaces.Services;
using Shop.Domain.Models;

namespace Shop.Application.Services;

public class UserAddressService(IUserAddressRepository _repository, IMapper _mapper) : IUserAddressService
{
    public async Task<UserAddressReadDTO> CreateAsync(Guid userId, UserAddressCreateDTO dto, CancellationToken cancellationToken = default)
    {
        if (userId == Guid.Empty)
            throw new ArgumentException("User id cannot be empty", nameof(userId));

        var address = new UserAddress
        {
            UserId = userId,
            Country = dto.Country.Trim(),
            City = dto.City.Trim(),
            Street = dto.Street.Trim(),
            Building = dto.Building.Trim(),
            Apartment = dto.Apartment?.Trim(),
            PostalCode = dto.PostalCode?.Trim()
        };

        await _repository.CreateAsync(address, cancellationToken);

        return _mapper.Map<UserAddressReadDTO>(address);
    }

    public async Task<IReadOnlyList<UserAddressReadDTO>> GetAllAsync(Guid userId, CancellationToken cancellationToken = default)
    {
        if (userId == Guid.Empty)
            throw new ArgumentException("User id cannot be empty", nameof(userId));

        var addresses = await _repository.GetByUserIdAsync(userId, cancellationToken);

        return _mapper.Map<List<UserAddressReadDTO>>(addresses);
    }

    public async Task<UserAddressReadDTO?> GetByIdAsync(Guid id, Guid userId, CancellationToken cancellationToken = default)
    {
        if (id == Guid.Empty || userId == Guid.Empty) return null;

        var address = await _repository.GetByIdAsync(id, userId, cancellationToken);
        if (address == null) return null;

        return _mapper.Map<UserAddressReadDTO>(address);
    }


    public async Task<bool> DeleteAsync(Guid id, Guid userId, CancellationToken cancellationToken = default)
    {
        if (id == Guid.Empty || userId == Guid.Empty) return false;

        return await _repository.DeleteAsync(id, userId, cancellationToken);
    }
}
