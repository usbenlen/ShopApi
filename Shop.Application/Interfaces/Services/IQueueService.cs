namespace Shop.Application.Interfaces.Services;

public interface IQueueService
{
    Task PublishAsync<T>(string queue, T message, CancellationToken cancellationToken = default);
}
