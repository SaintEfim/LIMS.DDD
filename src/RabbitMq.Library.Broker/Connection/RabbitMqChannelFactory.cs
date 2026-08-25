using Microsoft.Extensions.Logging;
using RabbitMQ.Client;

namespace RabbitMq.Library.Broker.Connection;

public sealed class RabbitMqChannelFactory
{
    private readonly ILogger<RabbitMqChannelFactory> _logger;

    public RabbitMqChannelFactory(
        RabbitMqConnectionProvider connectionService,
        ILogger<RabbitMqChannelFactory> logger)
    {
        ConnectionProvider = connectionService;
        _logger = logger;
    }

    private RabbitMqConnectionProvider ConnectionProvider { get; }

    public async Task<IChannel?> CreateChannelAsync(
        CancellationToken cancellationToken = default)
    {
        var connection = ConnectionProvider.Connection;

        if (connection is not null && connection.IsOpen)
        {
            var options = new CreateChannelOptions(true, true);

            return await connection.CreateChannelAsync(options, cancellationToken);
        }

        _logger.LogDebug("RabbitMQ connection is currently unavailable.");

        return null;
    }
}
