using RabbitMQ.Client;

namespace Guides.DDD.Service.Infrastructure.Messaging;

public sealed class RabbitMqChannelManager
{
    private readonly ILogger<RabbitMqChannelManager> _logger;

    private RabbitMqConnectionService ConnectionService { get; }

    public RabbitMqChannelManager(
        RabbitMqConnectionService connectionService,
        ILogger<RabbitMqChannelManager> logger)
    {
        ConnectionService = connectionService;
        _logger = logger;
    }

    public async Task<IChannel?> CreateChannelAsync(
        CancellationToken cancellationToken = default)
    {
        var connection = ConnectionService.Connection;

        if (connection is not null && connection.IsOpen)
        {
            return await connection.CreateChannelAsync(cancellationToken: cancellationToken);
        }

        _logger.LogDebug("RabbitMQ connection is currently unavailable.");

        return null;
    }
}
