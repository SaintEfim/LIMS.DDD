using System.Text;
using System.Text.Json;
using Guides.Messages;
using RabbitMQ.Client;

namespace Guides.DDD.Service.Infrastructure.Messaging;

public class RabbitMqMessageBus(RabbitMqChannelManager provider, ILogger<RabbitMqChannelManager> logger) : IMessageBus
{
    private static readonly JsonSerializerOptions JsonOptions = new()
    {
        PropertyNamingPolicy = JsonNamingPolicy.CamelCase,
        WriteIndented = false
    };

    public Task SendAsync<T>(
        T message,
        CancellationToken cancellationToken = default)
        where T : MessageBase
    {
        var json = JsonSerializer.Serialize(message, JsonOptions);
        return SendAsync(json, cancellationToken);
    }

    private async Task SendAsync(
        string message,
        CancellationToken cancellationToken = default)
    {
        await using var channel = await provider.CreateChannelAsync(cancellationToken);
        if (channel is null || !channel.IsOpen)
        {
            logger.LogWarning("RabbitMQ is not available. Message dropped: {Message}", message);
            return;
        }

        await channel.QueueDeclareAsync(queue: "unit_queue", durable: true, exclusive: false, autoDelete: false,
            arguments: null, cancellationToken: cancellationToken);

        var body = Encoding.UTF8.GetBytes(message);

        var properties = new BasicProperties
        {
            Persistent = true,
            ContentType = "application/json",
            MessageId = Guid.NewGuid()
                .ToString(),
            Timestamp = new AmqpTimestamp(DateTimeOffset.UtcNow.ToUnixTimeSeconds())
        };

        await channel.BasicPublishAsync(exchange: "", routingKey: "unit_queue", mandatory: false,
            basicProperties: properties, body: body, cancellationToken: cancellationToken);
    }
}
