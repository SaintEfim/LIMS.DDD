using Microsoft.Extensions.Logging;
using RabbitMQ.Client;

namespace RabbitMq.Library.QuickStart.Connection;

public sealed class RabbitMqChannelFactory
{
    private readonly ILogger<RabbitMqChannelFactory> _logger;

    private RabbitMqConnectionProvider ConnectionProvider { get; }

    public RabbitMqChannelFactory(
        RabbitMqConnectionProvider connectionService,
        ILogger<RabbitMqChannelFactory> logger)
    {
        ConnectionProvider = connectionService;
        _logger = logger;
    }

    public async Task<IChannel?> CreateChannelAsync(
        CancellationToken cancellationToken = default)
    {
        var connection = ConnectionProvider.Connection;

        if (connection is not null && connection.IsOpen)
        {
            var options = new CreateChannelOptions(publisherConfirmationsEnabled: true,
                publisherConfirmationTrackingEnabled: true, outstandingPublisherConfirmationsRateLimiter: null,
                consumerDispatchConcurrency: 1);

            return await connection.CreateChannelAsync(options, cancellationToken: cancellationToken);
        }

        _logger.LogDebug("RabbitMQ connection is currently unavailable.");

        return null;
    }
}
