using Microsoft.Extensions.Logging;
using Shop.Application.DTOs.UserDTOs;
using Shop.Application.Interfaces.Services;
using System.Text.Json;

public class UserMessageHandler(ILogger<UserMessageHandler> logger) : IRabbitMqMessageHandler
{
    public string QueueName => RabbitMqQueues.Users;

    public async Task HandleAsync(string json, CancellationToken cancellationToken = default)
    {
        var message = JsonSerializer.Deserialize<UserCreateDTO>(json);

        if (message is null)
        {
            logger.LogWarning("Invalid user message received");
            return;
        }

        logger.LogInformation($"User received. Email: {message.Email}");

        await Task.CompletedTask;
    }
}
