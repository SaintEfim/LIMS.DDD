using System.Reflection;
using System.Text;
using System.Text.Json;
using Guides.Messages;
using RabbitMQ.Client;

namespace Guides.DDD.Service.Infrastructure.Messaging;

public class RabbitMqMessageBus(
    RabbitMqChannelManager channelManager,
    ILogger<RabbitMqChannelManager> logger) : IMessageBus
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

        var typeObject = messageObject.GetType();

        var attribute = typeObject.GetCustomAttribute<IntegrationEventAttribute>();

        if (attribute == null)
            throw new InvalidOperationException($"Integration event {messageObject} has no IntegrationEventAttribute.");

        await channel.BasicPublishAsync(exchange: "", routingKey: attribute.QueueName, mandatory: false,
            basicProperties: properties, body: body, cancellationToken: cancellationToken);
    }
}
