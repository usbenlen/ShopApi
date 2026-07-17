
using Shop.Application.DTOs.AuthDTOs;
using Shop.Application.DTOs.UserDTOs;

namespace Shop.Application.Interfaces.Services;

public interface IAuthService
{
    Task<(UserReadDTO? User, string? AccessToken, RefreshTokenDTO? RefreshToken)> RegisterAsync(UserCreateDTO dto, CancellationToken cancellationToken = default);
    Task<(string AccessToken, RefreshTokenDTO RefreshToken)?> RefreshAsync(string token);
}
