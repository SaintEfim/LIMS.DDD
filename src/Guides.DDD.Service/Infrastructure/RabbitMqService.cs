using System.Text;
using System.Text.Json;
using RabbitMQ.Client;

namespace Guides.DDD.Service.Infrastructure;

public class RabbitMqService : IMessageBusService
{
    public async Task SendMessage(
        object obj,
        CancellationToken cancellationToken = default)
    {
        var message = JsonSerializer.Serialize(obj);
        await SendMessage(message, cancellationToken);
    }

    public async Task SendMessage(
        string message,
        CancellationToken cancellationToken = default)
    {
        var factory = new ConnectionFactory { HostName = "localhost" };
        await using var connection = await factory.CreateConnectionAsync(cancellationToken);
        await using var channel = await connection.CreateChannelAsync(cancellationToken: cancellationToken);

        await channel.QueueDeclareAsync(queue: "MyQueue", durable: false, exclusive: false, autoDelete: false,
            arguments: null, cancellationToken: cancellationToken);

        var body = Encoding.UTF8.GetBytes(message);

        var properties = new BasicProperties { Persistent = true };

        await channel.BasicPublishAsync(exchange: string.Empty, routingKey: "unit_queue", mandatory: true,
            basicProperties: properties, body: body, cancellationToken: cancellationToken);
    }
}
