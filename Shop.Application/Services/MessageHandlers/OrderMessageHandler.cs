using Microsoft.Extensions.Logging;
using Shop.Application.Constants;
using Shop.Application.DTOs.OrderDTOs;
using Shop.Application.Interfaces.Services;
using System.Text.Json;

namespace Shop.Application.Services.MessageHandlers;

public class OrderMessageHandler(ILogger<OrderMessageHandler> logger) : IRabbitMqMessageHandler
{
    public string QueueName => RabbitMqQueues.Orders;

    public async Task HandleAsync(string json, CancellationToken cancellationToken = default)
    {
        var message = JsonSerializer.Deserialize<OrderMessageDTO>(json);

        if (message is null)
        {
            logger.LogWarning("Invalid order message received");
            return;
        }

        logger.LogInformation($"Order received. UserId: {message.UserId}, TotalPrice: {message.TotalPrice}");

        foreach (var product in message.Products)
            logger.LogInformation($"ProductId: {product.ProductId}, Count: {product.Count}, Price: {product.Price}");

        await Task.CompletedTask;
    }
}
