namespace Guides.DDD.Service.Infrastructure.Messaging;

public class RabbitMqTopologyDeclarator(
    IReadOnlyCollection<IntegrationEventDescriptor> events,
    RabbitMqChannelManager channelManager,
    ILogger<RabbitMqTopologyDeclarator> logger)
{
    public async Task DeclareAllAsync(CancellationToken ct)
    {
        if (events.Count == 0)
        {
            logger.LogWarning("No integration events found");
            return;
        }

        await using var channel =
            await channelManager.CreateChannelAsync(ct);

        if (channel is null || !channel.IsOpen)
        {
            logger.LogWarning("RabbitMQ is not available.");
            return;
        }

        foreach (var @event in events)
        {
            await channel.QueueDeclareAsync(
                queue: @event.QueueName,
                durable: true,
                exclusive: false,
                autoDelete: false,
                arguments: null,
                cancellationToken: ct);

            logger.LogInformation(
                "Declared queue {Queue} for type {Type}",
                @event.QueueName,
                @event.EventType.Name);
        }

        logger.LogInformation(
            "RabbitMQ topology declared: {Count} queues",
            events.Count);
    }
}
