namespace Shop.Application.Interfaces.Services;

public interface IEmailService
{
    Task SendPasswordResetEmailAsync(string email, string token, CancellationToken cancellationToken = default);
    Task SendPasswordSetupEmailAsync(string email, string token, CancellationToken cancellationToken = default);
}