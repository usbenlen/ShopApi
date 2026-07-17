using AutoMapper;
using Shop.Application.DTOs.AuthDTOs;
using Shop.Application.DTOs.UserDTOs;
using Shop.Application.Interfaces.Helpers;
using Shop.Application.Interfaces.Repository;
using Shop.Application.Interfaces.Services;

namespace Shop.Application.Services;

public class AuthService(IMapper _mapper, IAuthRepository _repository, IHashHelper _hashHelper, IJWTService _jwtService, IRefreshTokenService _refreshTokenService, IRefreshTokenRepository _refreshTokenRepository) : IAuthService
{
    public async Task<(UserReadDTO? User, string? AccessToken, RefreshTokenDTO? RefreshToken)> RegisterAsync(UserCreateDTO dto, CancellationToken cancellationToken = default)
    {
        if (await _repository.IsEmailInUseAsync(dto.Email)) return (null, null, null);

        var user = _mapper.Map<User>(dto);
        user.PasswordHash = _hashHelper.Hash(dto.Password);

        await _repository.RegisterUserAsync(user);

        var accessToken = _jwtService.GenerateAccessToken(_mapper.Map<UserTokenDTO>(user));

        var refreshToken = _refreshTokenService.GenerateRefreshToken(user.Id);
        await _refreshTokenRepository.AddAsync(refreshToken);

        var refreshTokenDTO = new RefreshTokenDTO
        {
            Token = refreshToken.Token,
            ExpiresAt = refreshToken.ExpiresAt
        };

        return (_mapper.Map<UserReadDTO>(user), accessToken, refreshTokenDTO);
    }

    public async Task<(string AccessToken, RefreshTokenDTO RefreshToken)?> LoginAsync(UserLoginDTO dto)
    {
        var user = await _repository.GetByEmailAsync(dto.Email);
        if (user == null) return null;
        if (!_hashHelper.IsPasswordValid(dto.Password, user.PasswordHash)) return null;

        var accessToken = _jwtService.GenerateAccessToken(_mapper.Map<UserTokenDTO>(user));

        var refreshToken = _refreshTokenService.GenerateRefreshToken(user.Id);
        await _refreshTokenRepository.AddAsync(refreshToken);

        var refreshTokenDTO = new RefreshTokenDTO
        {
            Token = refreshToken.Token,
            ExpiresAt = refreshToken.ExpiresAt
        };

        return (accessToken, refreshTokenDTO);
    }

    public async Task<(string AccessToken, RefreshTokenDTO RefreshToken)?> RefreshAsync(string token)
    {
        var oldRefreshToken = await _refreshTokenRepository.GetByTokenAsync(token);
        if (oldRefreshToken == null || !oldRefreshToken.IsActive) return null;

        oldRefreshToken.IsRevoked = true;
        await _refreshTokenRepository.UpdateAsync(oldRefreshToken);

        var newRefreshToken = _refreshTokenService.GenerateRefreshToken(oldRefreshToken.UserId);
        await _refreshTokenRepository.AddAsync(newRefreshToken);

        var accessToken = _jwtService.GenerateAccessToken(_mapper.Map<UserTokenDTO>(oldRefreshToken.User));

        var refreshTokenDTO = new RefreshTokenDTO
        {
            Token = newRefreshToken.Token,
            ExpiresAt = newRefreshToken.ExpiresAt
        };

        return (accessToken, refreshTokenDTO);
    }
}
