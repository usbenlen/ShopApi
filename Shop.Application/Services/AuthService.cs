using AutoMapper;
using Shop.Application.DTOs.AuthDTOs;
using Shop.Application.DTOs.UserDTOs;
using Shop.Application.Interfaces.Helpers;
using Shop.Application.Interfaces.Repository;
using Shop.Application.Interfaces.Services;

namespace Shop.Application.Services;

public class AuthService(
    IMapper mapper,
    IAuthRepository repository,
    IHashHelper hashHelper,
    IJWTService jwtService,
    IRefreshTokenService refreshTokenService,
    IRefreshTokenRepository refreshTokenRepository)
    : IAuthService
{
    public async Task<(UserReadDTO? User, string? AccessToken, RefreshTokenDTO? RefreshToken)> RegisterAsync(UserCreateDTO dto, CancellationToken cancellationToken = default)
    {
        var email = NormalizeEmail(dto.Email);

        if (await repository.IsEmailInUseAsync(email))
            return (null, null, null);

        var user = mapper.Map<User>(dto);

        user.Email = email;
        user.PasswordHash = hashHelper.Hash(dto.Password);

        await repository.RegisterUserAsync(user);

        var accessToken = jwtService.GenerateAccessToken(mapper.Map<UserTokenDTO>(user));

        var refreshToken =refreshTokenService.GenerateRefreshToken(user.Id);

        await refreshTokenRepository.AddAsync(refreshToken);

        var refreshTokenDTO = new RefreshTokenDTO
        {
            Token = refreshToken.Token,
            ExpiresAt = refreshToken.ExpiresAt
        };

        return (
            mapper.Map<UserReadDTO>(user),
            accessToken,
            refreshTokenDTO);
    }

    public async Task<(string AccessToken, RefreshTokenDTO RefreshToken)?> LoginAsync(UserLoginDTO dto)
    {
        var email = NormalizeEmail(dto.Email);

        var user = await repository.GetByEmailAsync(email);

        if (user == null || !user.IsActive) return null;
        if (!hashHelper.IsPasswordValid(dto.Password,user.PasswordHash))
            return null;

        var accessToken = jwtService.GenerateAccessToken(mapper.Map<UserTokenDTO>(user));
        var refreshToken = refreshTokenService.GenerateRefreshToken(user.Id);

        await refreshTokenRepository.AddAsync(refreshToken);

        var refreshTokenDTO = new RefreshTokenDTO
        {
            Token = refreshToken.Token,
            ExpiresAt = refreshToken.ExpiresAt
        };

        return (accessToken, refreshTokenDTO);
    }

    public async Task<(string AccessToken, RefreshTokenDTO RefreshToken)?> RefreshAsync(string token)
    {
        if (string.IsNullOrWhiteSpace(token)) return null;

        var oldRefreshToken = await refreshTokenRepository.GetByTokenAsync(token);

        if (oldRefreshToken == null || !oldRefreshToken.IsActive || oldRefreshToken.User == null || !oldRefreshToken.User.IsActive)
            return null;

        oldRefreshToken.IsRevoked = true;

        await refreshTokenRepository.UpdateAsync(oldRefreshToken);

        var newRefreshToken = refreshTokenService.GenerateRefreshToken(oldRefreshToken.UserId);

        await refreshTokenRepository.AddAsync(newRefreshToken);

        var accessToken = jwtService.GenerateAccessToken(mapper.Map<UserTokenDTO>(oldRefreshToken.User));

        var refreshTokenDTO = new RefreshTokenDTO
        {
            Token = newRefreshToken.Token,
            ExpiresAt = newRefreshToken.ExpiresAt
        };

        return (accessToken, refreshTokenDTO);
    }

    private static string NormalizeEmail(string email)
    {
        return email.Trim().ToLowerInvariant();
    }
}