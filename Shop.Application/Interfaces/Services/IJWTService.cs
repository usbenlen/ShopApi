using Shop.Application.DTOs.UserDTOs;
using Shop.Domain.Models;

namespace Shop.Application.Interfaces.Services;

public interface IJWTService
{
    string GenerateAccessToken(UserTokenDTO user);
}
