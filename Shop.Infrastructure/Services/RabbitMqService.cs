using Microsoft.Extensions.Options;
using RabbitMQ.Client;
using Shop.Application.Interfaces.Services;
using Shop.Infrastructure.Configuration;
using System.Text;
using System.Text.Json;

namespace Shop.Infrastructure.Services;

public class RabbitMqService(IOptions<RabbitMqSettings> options) : IQueueService, IAsyncDisposable
{
    private readonly RabbitMqSettings _settings = options.Value;

    private IConnection? _connection;
    private IChannel? _channel;

    private readonly SemaphoreSlim _initializationLock = new(1, 1);

    /// <summary>
    /// Ініціалізує RabbitMQ connection та channel
    /// Створюються тільки один раз
    /// </summary>
    private async Task EnsureInitializedAsync(CancellationToken cancellationToken)
    {
        if (_connection is not null && _connection.IsOpen && _channel is not null && _channel.IsOpen)
            return;

        await _initializationLock.WaitAsync(cancellationToken);

        try
        {
            // Повторна перевірка після отримання lock
            if (_connection is not null && _connection.IsOpen && _channel is not null && _channel.IsOpen)
                return;

            // Якщо старі дані є, але вже закриті чистимо їх перед створенням нових
            if (_channel is not null)
            {
                await _channel.DisposeAsync();
                _channel = null;
            }

            if (_connection is not null)
            {
                await _connection.DisposeAsync();
                _connection = null;
            }

            var factory = new ConnectionFactory
            {
                HostName = _settings.Host,
                Port = _settings.Port
            };

            _connection = await factory.CreateConnectionAsync(cancellationToken);
            _channel = await _connection.CreateChannelAsync(cancellationToken: cancellationToken);
        }
        finally
        {
            _initializationLock.Release();
        }
    }

    /// <summary>
    /// Відправляє повідомлення у RabbitMQ queue
    /// </summary>
    public async Task PublishAsync<T>(string queue, T message, CancellationToken cancellationToken = default)
    {
        cancellationToken.ThrowIfCancellationRequested();

        await EnsureInitializedAsync(cancellationToken);

        if (_channel is null || !_channel.IsOpen)
            throw new InvalidOperationException("RabbitMQ channel is not available");

        // Переконуємося що черга існує
        await _channel.QueueDeclareAsync(
            queue: queue,
            durable: true,
            exclusive: false,
            autoDelete: false,
            arguments: null,
            cancellationToken: cancellationToken);

        var json = JsonSerializer.Serialize(message);

        var body = Encoding.UTF8.GetBytes(json);

        var properties = new BasicProperties
        {
            Persistent = true
        };

        await _channel.BasicPublishAsync(
            exchange: "",
            routingKey: queue,
            mandatory: false,
            basicProperties: properties,
            body: body,
            cancellationToken: cancellationToken);
    }

    /// <summary>
    /// Закриває RabbitMQ channel та connection
    /// </summary>
    public async ValueTask DisposeAsync()
    {
        _initializationLock.Dispose();

        if (_channel is not null)
        {
            await _channel.DisposeAsync();
            _channel = null;
        }

        if (_connection is not null)
        {
            await _connection.DisposeAsync();
            _connection = null;
        }
    }
}
