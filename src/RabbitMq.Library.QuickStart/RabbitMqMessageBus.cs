using System.Text;
using System.Text.Json;
using Microsoft.Extensions.Logging;
using RabbitMQ.Client;
using RabbitMq.Library.QuickStart.Abstractions;
using RabbitMq.Library.QuickStart.Connection;
using RabbitMq.Library.QuickStart.IntegrationEvents;
using RabbitMq.Library.QuickStart.Messages;

namespace RabbitMq.Library.QuickStart;

public sealed class RabbitMqMessageBus(
    IntegrationEventRegistry eventRegistry,
    RabbitMqChannelFactory channelFactory,
    ILogger<RabbitMqMessageBus> logger) : IMessageBus
{
    private static readonly JsonSerializerOptions JsonOptions =
        new()
        {
            PropertyNamingPolicy = JsonNamingPolicy.CamelCase,
            WriteIndented = false
        };

    public async Task SendAsync<T>(
        T message,
        CancellationToken cancellationToken = default)
        where T : IIntegrationEvent
    {
        var descriptor = eventRegistry.Get<T>();
        var json = JsonSerializer.Serialize(message, JsonOptions);
        await using var channel = await channelFactory.CreateChannelAsync(cancellationToken);
        if (channel is null || !channel.IsOpen)
        {
            logger.LogWarning("RabbitMQ is not available. Message dropped: {EventType}", typeof(T).Name);
            return;
        }

        var body = Encoding.UTF8.GetBytes(json);
        var properties = new BasicProperties
        {
            Persistent = true,
            ContentType = "application/json",
            MessageId = Guid.NewGuid()
                .ToString(),
            Timestamp = new AmqpTimestamp(DateTimeOffset.UtcNow.ToUnixTimeSeconds())
        };
        await channel.BasicPublishAsync("", descriptor.QueueName, false, properties, body, cancellationToken);
    }
}
