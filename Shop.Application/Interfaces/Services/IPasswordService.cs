using Shop.Application.DTOs.AuthDTOs;

namespace Shop.Application.Interfaces.Services;

public interface IPasswordService
{
    Task RequestPasswordResetAsync(string email, CancellationToken cancellationToken = default);
    Task<bool> ResetPasswordAsync(ResetPasswordDTO dto, CancellationToken cancellationToken = default);
    Task<bool> ChangePasswordAsync(Guid userId, ChangePasswordDTO dto, CancellationToken cancellationToken = default);
    Task<bool> SendPasswordSetupAsync(User user, CancellationToken cancellationToken = default);
}