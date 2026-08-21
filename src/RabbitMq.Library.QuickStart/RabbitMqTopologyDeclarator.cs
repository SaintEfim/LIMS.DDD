using Microsoft.Extensions.Logging;
using RabbitMq.Library.QuickStart.Connection;
using RabbitMq.Library.QuickStart.IntegrationEvents;

namespace RabbitMq.Library.QuickStart;

public sealed class RabbitMqTopologyDeclarator(
    IntegrationEventRegistry eventRegistry,
    RabbitMqChannelFactory channelFactory,
    ILogger<RabbitMqTopologyDeclarator> logger)
{
    public async Task DeclareAllAsync(CancellationToken cancellationToken = default)
    {
        await using var channel = await channelFactory.CreateChannelAsync(cancellationToken);
        if (channel is null || !channel.IsOpen) return;

        var declaredExchanges = new HashSet<string>();

        foreach (var descriptor in eventRegistry.All.Values)
        {
            if (declaredExchanges.Add(descriptor.ExchangeName))
            {
                await channel.ExchangeDeclareAsync(
                    exchange: descriptor.ExchangeName,
                    type: "fanout",
                    durable: true,
                    autoDelete: false,
                    arguments: null,
                    cancellationToken: cancellationToken);

                logger.LogInformation("Declared fanout exchange: {ExchangeName}", descriptor.ExchangeName);
            }

            await channel.QueueDeclareAsync(
                queue: descriptor.QueueName,
                durable: true,
                exclusive: false,
                autoDelete: false,
                arguments: null,
                cancellationToken: cancellationToken);

            await channel.QueueBindAsync(
                queue: descriptor.QueueName,
                exchange: descriptor.ExchangeName,
                routingKey: "",
                arguments: null,
                cancellationToken: cancellationToken);

            logger.LogInformation(
                "Declared and bound queue: {QueueName} -> exchange: {ExchangeName}",
                descriptor.QueueName, descriptor.ExchangeName);
        }
    }
}
