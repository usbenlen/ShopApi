using Shop.Application.Constants;
using Shop.Application.DTOs.OrderDTOs;
using Shop.Application.Interfaces.Repository;
using Shop.Application.Interfaces.Services;

namespace Shop.Application.Services;

public class OrderService(IOrderRepository orderRepository, IUserRepository userRepository, IEmailService emailService, ICachingService cache) : IOrderService
{
    public async Task ProcessOrderAsync(OrderMessageDTO message, CancellationToken cancellationToken = default)
    {
        if (message.OrderRequestId == Guid.Empty)
            throw new ArgumentException("Order request id cannot be empty");

        if (message.UserId == Guid.Empty)
            throw new ArgumentException("User id cannot be empty");

        if (message.Products is null || message.Products.Count == 0)
            throw new ArgumentException("Order must contain at least one product");

        foreach (var product in message.Products)
        {
            if (product.ProductId <= 0)
                throw new ArgumentException($"Invalid product id: {product.ProductId}");

            if (product.Count <= 0)
                throw new ArgumentException($"Invalid count for product {product.ProductId}");
        }

        var result = await orderRepository.CreateOrderAsync(
            message.OrderRequestId,
            message.UserId,
            message.Products,
            cancellationToken);

        if (result.AlreadyProcessed) return;

        var email = await userRepository.GetEmailAsync(message.UserId, cancellationToken);

        if (result.WaitingForStock)
        {
            await emailService.SendOrderWaitingEmailAsync(
                email,
                result.UnavailableProducts,
                cancellationToken);

            return;
        }

        if (!result.Success)
            throw new InvalidOperationException("Order could not be processed");

        // кількість товару змінилась, чистим product cache
        await cache.InvalidateGroupAsync(CacheKeys.ProductsGroup, cancellationToken);

        await emailService.SendOrderConfirmationEmailAsync(
            email,
            result.Products,
            result.TotalPrice,
            result.OrderId!.Value,
            cancellationToken);
    }
}
