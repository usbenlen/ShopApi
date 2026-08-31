using System.Text.Json;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Shop.Application.Constants;
using Shop.Application.DTOs.OrderDTOs;
using Shop.Application.Interfaces.Services;

namespace Shop.Application.Services.MessageHandlers;

public class OrderMessageHandler(ILogger<OrderMessageHandler> logger, IServiceScopeFactory scopeFactory) : IRabbitMqMessageHandler
{
    public string QueueName => RabbitMqQueues.Orders;

    public async Task HandleAsync(string json, CancellationToken cancellationToken = default)
    {
        OrderMessageDTO? message;

        try
        {
            message = JsonSerializer.Deserialize<OrderMessageDTO>(json);
        }
        catch (JsonException ex)
        {
            logger.LogError(ex, "Invalid JSON received from Orders queue");
            throw;
        }

        if (message is null)
            throw new InvalidOperationException("Order message is null");

        logger.LogInformation($"Order received. OrderRequestId: {message.OrderRequestId}, UserId: {message.UserId}");

        using var scope = scopeFactory.CreateAsyncScope();

        var orderService = scope.ServiceProvider.GetRequiredService<IOrderService>();
        await orderService.ProcessOrderAsync(message, cancellationToken);

        logger.LogInformation($"Order processed. OrderRequestId: {message.OrderRequestId}, UserId: {message.UserId}");
    }
}
