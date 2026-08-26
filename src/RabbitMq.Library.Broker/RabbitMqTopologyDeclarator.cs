using Microsoft.Extensions.Logging;
using RabbitMq.Library.Broker.Connection;
using RabbitMq.Library.Broker.IntegrationEvents;

namespace RabbitMq.Library.Broker;

public sealed class RabbitMqTopologyDeclarator(
    IntegrationEventRegistry eventRegistry,
    RabbitMqChannelFactory channelFactory,
    ILogger<RabbitMqTopologyDeclarator> logger)
{
    public async Task DeclareAllAsync(
        CancellationToken cancellationToken = default)
    {
        await using var channel = await channelFactory.CreateChannelAsync(cancellationToken);
        if (channel is null || !channel.IsOpen)
        {
            return;
        }

        var declaredExchanges = new HashSet<string>();

        foreach (var descriptor in eventRegistry.All.Values)
        {
            if (declaredExchanges.Add(descriptor.ExchangeName))
            {
                await channel.ExchangeDeclareAsync(descriptor.ExchangeName, "fanout", true, false, null,
                    cancellationToken: cancellationToken);

                logger.LogInformation("Declared fanout exchange: {ExchangeName}", descriptor.ExchangeName);
            }

            await channel.QueueDeclareAsync(descriptor.QueueName, true, false, false, null,
                cancellationToken: cancellationToken);

            await channel.QueueBindAsync(descriptor.QueueName, descriptor.ExchangeName, "", null,
                cancellationToken: cancellationToken);

            logger.LogInformation("Declared and bound queue: {QueueName} -> exchange: {ExchangeName}",
                descriptor.QueueName, descriptor.ExchangeName);
        }
    }
}
