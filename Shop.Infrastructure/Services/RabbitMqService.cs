using Microsoft.Extensions.Options;
using Shop.Application.Interfaces.Services;
using Shop.Infrastructure.Configuration;
using RabbitMQ.Client;
using System.Text;
using System.Text.Json;

namespace Shop.Infrastructure.Services;

public class RabbitMqService(IOptions<RabbitMqSettings> options) : IQueueService
{
    private readonly RabbitMqSettings _settings = options.Value;

    // Метод для відправки повідомлення у чергу
    public async Task PublishAsync<T>(string queue, T message, CancellationToken cancellationToken = default)
    {
        cancellationToken.ThrowIfCancellationRequested();

        // Створюємо фабрику підключення до RabbitMQ
        var factory = new ConnectionFactory()
        {
            // Host сервера RabbitMQ
            HostName = _settings.Host,

            // Порт RabbitMQ (зазвичай 5672)
            Port = _settings.Port
        };

        // Створюємо з'єднання з RabbitMQ сервером
        await using var connection = await factory.CreateConnectionAsync(cancellationToken);

        // Створюємо канал (channel) для роботи з чергами
        await using var channel = await connection.CreateChannelAsync(cancellationToken: cancellationToken);

        // Оголошуємо чергу
        // Якщо черга не існує — вона буде створена
        await channel.QueueDeclareAsync(
            queue: queue,          // назва черги
            durable: true,         // черга зберігається після перезапуску RabbitMQ
            exclusive: false,      // доступна для інших з'єднань
            autoDelete: false,     // не видаляється автоматично
            arguments: null,        // додаткові параметри
            cancellationToken: cancellationToken
        );

        // Серіалізуємо повідомлення у JSON
        var json = JsonSerializer.Serialize(message);

        // Перетворюємо JSON у масив байтів
        // RabbitMQ передає повідомлення саме у вигляді байтів
        var body = Encoding.UTF8.GetBytes(json);

        // Властивості повідомлення
        var properties = new BasicProperties
        {
            // Робить повідомлення persistent (зберігається на диску)
            Persistent = true
        };

        // Відправляємо повідомлення у чергу
        await channel.BasicPublishAsync(
             exchange: "",        // стандартний exchange
             routingKey:queue,   // назва черги (routing key)
             mandatory: false,    // якщо черга не знайдена — повідомлення просто ігнорується
             basicProperties: properties,
             body: body,           // тіло повідомлення
             cancellationToken: cancellationToken
        );
    }
}
