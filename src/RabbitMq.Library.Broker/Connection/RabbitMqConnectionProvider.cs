using Microsoft.Extensions.Logging;
using RabbitMQ.Client;
using RabbitMQ.Client.Events;

namespace RabbitMq.Library.Broker.Connection;

public class RabbitMqConnectionProvider : IAsyncDisposable
{
    private readonly CancellationTokenSource _disposeCts = new();
    private readonly ILogger<RabbitMqConnectionProvider> _logger;

    private readonly RabbitMqOptions _options;
    private readonly SemaphoreSlim _semaphore = new(1, 1);

    public RabbitMqConnectionProvider(
        RabbitMqOptions options,
        ILogger<RabbitMqConnectionProvider> logger)
    {
        _options = options;
        _logger = logger;

        ConnectionFactory = new ConnectionFactory
        {
            HostName = options.HostName,
            Port = options.Port,
            UserName = options.UserName,
            Password = options.Password,
            VirtualHost = options.VirtualHost,
            RequestedHeartbeat = options.RequestedHeartbeat,
            AutomaticRecoveryEnabled = options.AutomaticRecoveryEnabled,
            TopologyRecoveryEnabled = options.TopologyRecoveryEnabled,
            NetworkRecoveryInterval = options.NetworkRecoveryInterval
        };

        _logger.LogInformation("RabbitMQ connection factory initialized for host {Host}:{Port}",
            ConnectionFactory.HostName, ConnectionFactory.Port);
    }

    public IConnection? Connection { get; private set; }
    private ConnectionFactory ConnectionFactory { get; }

    public async ValueTask DisposeAsync()
    {
        _logger.LogInformation("Disposing RabbitMQ connection factory...");

        await _disposeCts.CancelAsync();
        await CleanUpAsync();
        _semaphore.Dispose();
        _disposeCts.Dispose();

        _logger.LogInformation("RabbitMQ connection factory disposed");
    }

    public async Task<bool> EnsureConnectedAsync(
        CancellationToken cancellationToken = default)
    {
        if (Connection is not null && Connection.IsOpen)
        {
            return false;
        }

        await ReconnectAsync(cancellationToken);

        if (Connection is null || !Connection.IsOpen)
        {
            throw new InvalidOperationException("RabbitMQ connection is not available after reconnection attempts.");
        }

        return true;
    }

    private async Task ReconnectAsync(
        CancellationToken cancellationToken = default)
    {
        using var linkedCts = CancellationTokenSource.CreateLinkedTokenSource(cancellationToken, _disposeCts.Token);

        await _semaphore.WaitAsync(linkedCts.Token);

        try
        {
            if (Connection is not null && Connection.IsOpen)
            {
                return;
            }

            await CleanUpAsync(linkedCts.Token);

            for (var attempt = 1; attempt <= _options.MaxReconnectAttempts; attempt++)
            {
                try
                {
                    _logger.LogDebug("Creating new RabbitMQ connection. Attempt {Attempt}/{MaxAttempts}", attempt,
                        _options.MaxReconnectAttempts);

                    var connection = await ConnectionFactory.CreateConnectionAsync(linkedCts.Token);

                    connection.ConnectionShutdownAsync += OnConnectionShutdownAsync;

                    Connection = connection;

                    _logger.LogInformation("RabbitMQ connection established successfully");

                    return;
                }
                catch (OperationCanceledException) when (linkedCts.IsCancellationRequested)
                {
                    throw;
                }
                catch (Exception ex)
                {
                    var delay = Math.Min((int) Math.Pow(_options.BackoffBaseSeconds, attempt),
                        _options.MaxBackoffDelaySeconds);

                    _logger.LogWarning(ex,
                        "RabbitMQ connection attempt {Attempt}/{MaxAttempts} failed. " + "Next retry in {Delay}s",
                        attempt, _options.MaxReconnectAttempts, delay);

                    await Task.Delay(TimeSpan.FromSeconds(delay), linkedCts.Token);
                }
            }
        }
        finally
        {
            _semaphore.Release();
        }
    }

    private async Task OnConnectionShutdownAsync(
        object sender,
        ShutdownEventArgs ev)
    {
        _logger.LogWarning("RabbitMQ connection shut down. Reason: {Reason}, Initiator: {Initiator}", ev.ReplyText,
            ev.Initiator);

        await ReconnectAsync(_disposeCts.Token);
    }

    private async Task CleanUpAsync(
        CancellationToken cancellationToken = default)
    {
        var connection = Connection;

        if (connection is null)
        {
            Connection = null;
            return;
        }

        Connection = null;

        connection.ConnectionShutdownAsync -= OnConnectionShutdownAsync;

        try
        {
            if (connection.IsOpen)
            {
                _logger.LogDebug("Closing RabbitMQ connection gracefully...");

                await connection.CloseAsync(cancellationToken);
            }
        }
        catch (IOException ex)
        {
            _logger.LogWarning(ex, "IOException during connection cleanup " + "(expected if connection is dead)");
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Unexpected error during connection cleanup");
        }
    }
}
