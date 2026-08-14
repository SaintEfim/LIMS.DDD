using System.Text;
using System.Text.Json;
using Guides.Messages;
using RabbitMQ.Client;

namespace Guides.DDD.Service.Infrastructure.Messaging;

public class RabbitMqMessageBus(
    IIntegrationEventRegistry eventRegistry,
    RabbitMqChannelFactory channelManager,
    ILogger<RabbitMqMessageBus> logger) : IMessageBus
{
    private static readonly JsonSerializerOptions JsonOptions = new()
    {
        PropertyNamingPolicy = JsonNamingPolicy.CamelCase,
        WriteIndented = false
    };

    public async Task SendAsync<T>(
        T messageObject,
        CancellationToken cancellationToken = default)
        where T : IIntegrationEvent
    {
        var messageString = JsonSerializer.Serialize(messageObject, JsonOptions);

        await using var channel = await channelManager.CreateChannelAsync(cancellationToken);
        if (channel is null || !channel.IsOpen)
        {
            logger.LogWarning("RabbitMQ is not available. Message dropped: {Message}", messageString);
            return;
        }

        var body = Encoding.UTF8.GetBytes(messageString);

        var properties = new BasicProperties
        {
            Persistent = true,
            ContentType = "application/json",
            MessageId = Guid.NewGuid()
                .ToString(),
            Timestamp = new AmqpTimestamp(DateTimeOffset.UtcNow.ToUnixTimeSeconds())
        };

        var descriptor = eventRegistry.Get<T>();

        await channel.BasicPublishAsync(exchange: "", routingKey: descriptor.QueueName, mandatory: false,
            basicProperties: properties, body: body, cancellationToken: cancellationToken);
    }
}
