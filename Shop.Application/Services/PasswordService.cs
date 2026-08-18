using Shop.Application.DTOs.AuthDTOs;
using Shop.Application.Helpers;
using Shop.Application.Interfaces.Helpers;
using Shop.Application.Interfaces.Repository;
using Shop.Application.Interfaces.Services;
using Shop.Domain.Models;

namespace Shop.Application.Services;

public class PasswordService(
    IUserRepository userRepository,
    IPasswordResetTokenRepository resetTokenRepository,
    IRefreshTokenRepository refreshTokenRepository,
    IHashHelper hashHelper,
    IEmailService emailService) : IPasswordService
{
    public async Task RequestPasswordResetAsync(string email, CancellationToken cancellationToken = default)
    {
        var normalizedEmail = NormalizeEmail(email);

        var user = await userRepository.GetByEmailAsync(normalizedEmail, cancellationToken);

        if (user == null) return;

        await resetTokenRepository.InvalidateActiveTokensAsync(user.Id, cancellationToken);

        var token = PasswordResetTokenHelper.GenerateToken();
        var tokenHash = PasswordResetTokenHelper.HashToken(token);

        var resetToken = new PasswordResetToken
        {
            UserId = user.Id,
            TokenHash = tokenHash,
            ExpiresAt = DateTime.UtcNow.AddMinutes(30),
            UsedAt = null
        };

        await resetTokenRepository.CreateAsync(resetToken, cancellationToken);

        await emailService.SendPasswordResetEmailAsync(
            user.Email,
            token,
            cancellationToken);
    }

    public async Task<bool> ResetPasswordAsync(ResetPasswordDTO dto, CancellationToken cancellationToken = default)
    {
        if (string.IsNullOrWhiteSpace(dto.Token)) return false;

        if (string.IsNullOrWhiteSpace(dto.NewPassword)) return false;

        var tokenHash = PasswordResetTokenHelper.HashToken(dto.Token);

        var resetToken = await resetTokenRepository.GetActiveByHashAsync(tokenHash, cancellationToken);

        if (resetToken == null) return false;

        var user = resetToken.User;

        if (!user.IsActive) return false;

        user.PasswordHash = hashHelper.Hash(dto.NewPassword);

        resetToken.UsedAt = DateTime.UtcNow;

        await userRepository.UpdateAsync(user, cancellationToken);

        await resetTokenRepository.UpdateAsync(resetToken, cancellationToken);

        await refreshTokenRepository.RevokeAllForUserAsync(user.Id, cancellationToken);

        return true;
    }

    public async Task<bool> ChangePasswordAsync(
        Guid userId,
        ChangePasswordDTO dto,
        CancellationToken cancellationToken = default)
    {
        var user = await userRepository.GetByIdAsync(userId, cancellationToken);

        if (user == null || !user.IsActive) return false;

        if (!hashHelper.IsPasswordValid(dto.CurrentPassword, user.PasswordHash))
            return false;

        if (dto.CurrentPassword == dto.NewPassword) return false;

        user.PasswordHash = hashHelper.Hash(dto.NewPassword);

        await userRepository.UpdateAsync(user, cancellationToken);

        await refreshTokenRepository.RevokeAllForUserAsync(user.Id, cancellationToken);

        return true;
    }

    public async Task<bool> SendPasswordSetupAsync(User user, CancellationToken cancellationToken = default)
    {
        if (user == null || !user.IsActive) return false;

        await resetTokenRepository.InvalidateActiveTokensAsync(user.Id, cancellationToken);

        var token = PasswordResetTokenHelper.GenerateToken();
        var tokenHash = PasswordResetTokenHelper.HashToken(token);

        var resetToken = new PasswordResetToken
        {
            UserId = user.Id,
            TokenHash = tokenHash,
            ExpiresAt = DateTime.UtcNow.AddMinutes(30),
            UsedAt = null
        };

        await resetTokenRepository.CreateAsync(resetToken, cancellationToken);

        await emailService.SendPasswordSetupEmailAsync(
            user.Email,
            token,
            cancellationToken);

        return true;
    }

    private static string NormalizeEmail(string email)
    {
        return email.Trim().ToLowerInvariant();
    }
}