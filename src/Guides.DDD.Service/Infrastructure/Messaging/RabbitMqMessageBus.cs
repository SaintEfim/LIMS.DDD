using System.Text;
using System.Text.Json;
using RabbitMQ.Client;

namespace Guides.DDD.Service.Infrastructure.Messaging;

public class RabbitMqMessageBus(RabbitMqChannelManager provider, ILogger<RabbitMqChannelManager> logger) : IMessageBus
{
    public async Task Send(
        object obj,
        CancellationToken cancellationToken = default)
    {
        var message = JsonSerializer.Serialize(obj);
        await Send(message, cancellationToken);
    }

    public async Task Send(
        string message,
        CancellationToken cancellationToken = default)
    {
        await using var channel = await provider.CreateChannelAsync(cancellationToken);
        if (channel is null || !channel.IsOpen)
        {
            logger.LogWarning("RabbitMQ is not available. Message dropped: {Message}", message);
            return;
        }

        await channel.QueueDeclareAsync(queue: "unit_queue", durable: false, exclusive: false, autoDelete: false,
            arguments: null, cancellationToken: cancellationToken);

        var body = Encoding.UTF8.GetBytes(message);

        var properties = new BasicProperties { Persistent = true };

        await channel.BasicPublishAsync(exchange: string.Empty, routingKey: "unit_queue", mandatory: true,
            basicProperties: properties, body: body, cancellationToken: cancellationToken);
    }
}
