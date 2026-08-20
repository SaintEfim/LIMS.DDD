using Microsoft.Extensions.Logging;
using RabbitMq.Library.QuickStart.Connection;
using RabbitMq.Library.QuickStart.IntegrationEvents;

namespace RabbitMq.Library.QuickStart;

public sealed class RabbitMqTopologyDeclarator(
    IntegrationEventRegistry eventRegistry,
    RabbitMqChannelFactory channelFactory,
    ILogger<RabbitMqTopologyDeclarator> logger)
{
    public async Task DeclareAsync(
        CancellationToken cancellationToken = default)
    {
        await using var channel = await channelFactory.CreateChannelAsync(cancellationToken);
        if (channel is null || !channel.IsOpen)
        {
            return;
        }

        foreach (var descriptor in eventRegistry.All.Values)
        {
            await channel.QueueDeclareAsync(descriptor.QueueName, true, false, false, null,
                cancellationToken: cancellationToken);

            logger.LogInformation("Declared queue: {QueueName}", descriptor.QueueName);
        }
    }
}
