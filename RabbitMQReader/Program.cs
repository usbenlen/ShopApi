using Microsoft.Extensions.Configuration;
using RabbitMQ.Client;
using RabbitMQ.Client.Events;
using System.Text;
using System.Text.Json;

namespace RabbitMQReader;

sealed class User
{
    public string Email { get; set; } = string.Empty;
    public string Password { get; set; } = string.Empty;
}

internal class Program
{
    static async Task Main(string[] args)
    {
        var configuration = new ConfigurationBuilder()
            .SetBasePath(AppContext.BaseDirectory)
            .AddJsonFile("appsettings.json", optional: false)
            .Build();

        var rabbitMq = configuration.GetSection("RabbitMq");

        var host = rabbitMq["Host"] ?? throw new InvalidOperationException("RabbitMq:Host is not configured");
        var port = rabbitMq.GetValue<int>("Port");
        var queue = rabbitMq["Queue"] ?? throw new InvalidOperationException("RabbitMq:Queue is not configured");

        var factory = new ConnectionFactory
        {
            HostName = host,
            Port = port
        };

        await using var connection = await factory.CreateConnectionAsync();

        await using var channel = await connection.CreateChannelAsync();

        await channel.QueueDeclareAsync(
            queue: queue,
            durable: true,
            exclusive: false,
            autoDelete: false,
            arguments: null);

        var consumer = new AsyncEventingBasicConsumer(channel);

        consumer.ReceivedAsync += async (sender, e) =>
        {
            var body = e.Body.ToArray();

            var json = Encoding.UTF8.GetString(body);

            var message = JsonSerializer.Deserialize<User>(json);

            if (message is null)
            {
                Console.WriteLine("Invalid message");
                return;
            }

            Console.WriteLine("\n=================================\n");
            Console.WriteLine("Message received");
            Console.WriteLine($"Email: {message.Email}");
            Console.WriteLine($"Password: {message.Password}");
            Console.WriteLine("=================================\n");

            await Task.CompletedTask;
        };

        await channel.BasicConsumeAsync(
            queue: queue,
            autoAck: true,
            consumer: consumer);

        Console.WriteLine($"Waiting messages from queue '{queue}'...");
        Console.ReadLine();
    }
}