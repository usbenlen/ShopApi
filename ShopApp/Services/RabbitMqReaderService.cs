using Microsoft.Extensions.Options;
using RabbitMQ.Client;
using RabbitMQ.Client.Events;
using Shop.Application.Interfaces.Services;
using Shop.Infrastructure.Configuration;
using System.Text;

namespace Shop.Api.Services;

/// <summary>
/// Універсальний RabbitMQ reader
/// Підписується на всі зареєстровані handlers
/// </summary>
public class RabbitMqReaderService(
    ILogger<RabbitMqReaderService> logger,
    IOptions<RabbitMqSettings> options,
    IEnumerable<IRabbitMqMessageHandler> handlers) : BackgroundService
{
    private readonly ILogger<RabbitMqReaderService> _logger = logger;
    private readonly RabbitMqSettings _settings = options.Value;
    private readonly IEnumerable<IRabbitMqMessageHandler> _handlers = handlers;

    private IConnection? _connection;
    private IChannel? _channel;

    protected override async Task ExecuteAsync(
        CancellationToken stoppingToken)
    {
        var factory = new ConnectionFactory
        {
            HostName = _settings.Host,
            Port = _settings.Port
        };

        _connection = await factory.CreateConnectionAsync(stoppingToken);

        _channel = await _connection.CreateChannelAsync(cancellationToken: stoppingToken);

        foreach (var handler in _handlers) await SubscribeAsync(handler, stoppingToken);

        _logger.LogInformation(
            "RabbitMQ Reader started. Queues: {Queues}",
            string.Join(", ", _handlers.Select(x => x.QueueName)));

        try
        {
            await Task.Delay(Timeout.Infinite, stoppingToken);
        }
        catch (OperationCanceledException)
        {
            // Нормальне завершення BackgroundService
        }
    }

    private async Task SubscribeAsync(IRabbitMqMessageHandler handler, CancellationToken cancellationToken)
    {
        if (_channel is null)
            throw new InvalidOperationException("RabbitMQ channel is not initialized");

        await _channel.QueueDeclareAsync(
            queue: handler.QueueName,
            durable: true,
            exclusive: false,
            autoDelete: false,
            arguments: null,
            cancellationToken: cancellationToken);

        var consumer = new AsyncEventingBasicConsumer(_channel);

        consumer.ReceivedAsync += async (_, eventArgs) =>
        {
            try
            {
                var body = eventArgs.Body.ToArray();

                var json = Encoding.UTF8.GetString(body);

                await handler.HandleAsync(json, cancellationToken);
            }
            catch (Exception ex)
            {
                _logger.LogError(
                    ex,
                    $"Error while processing message from queue {handler.QueueName}");
            }
        };

        await _channel.BasicConsumeAsync(
            queue: handler.QueueName,
            autoAck: true,
            consumer: consumer,
            cancellationToken: cancellationToken);

        _logger.LogInformation($"RabbitMQ subscribed to queue: {handler.QueueName}");
    }

    public override async Task StopAsync(CancellationToken cancellationToken)
    {
        _logger.LogInformation("RabbitMQ Reader stopping...");

        if (_channel != null) await _channel.CloseAsync(cancellationToken);
        if (_connection != null) await _connection.CloseAsync(cancellationToken);

        await base.StopAsync(cancellationToken);
    }
}
