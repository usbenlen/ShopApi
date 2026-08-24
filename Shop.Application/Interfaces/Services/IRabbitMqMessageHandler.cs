namespace Shop.Application.Interfaces.Services;

public interface IRabbitMqMessageHandler
{
    string QueueName { get; }

    Task HandleAsync(string json, CancellationToken cancellationToken = default);
}
