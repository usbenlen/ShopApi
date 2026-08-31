using Shop.Application.DTOs.OrderDTOs;

namespace Shop.Application.Interfaces.Services;

public interface IEmailService
{
    Task SendPasswordResetEmailAsync(string email, string token, CancellationToken cancellationToken = default);
    Task SendPasswordSetupEmailAsync(string email, string token, CancellationToken cancellationToken = default);
    Task SendOrderConfirmationEmailAsync(string email, IReadOnlyList<OrderProcessingProductDTO> products, decimal totalPrice, int orderId, CancellationToken cancellationToken = default);
    Task SendOrderWaitingEmailAsync(string email, IReadOnlyList<OrderProcessingProductDTO> unavailableProducts, CancellationToken cancellationToken = default);
}