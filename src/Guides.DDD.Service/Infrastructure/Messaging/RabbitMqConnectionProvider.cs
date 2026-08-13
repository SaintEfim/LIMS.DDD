using RabbitMQ.Client;
using RabbitMQ.Client.Events;

namespace Guides.DDD.Service.Infrastructure.Messaging;

public sealed class RabbitMqConnectionProvider : IAsyncDisposable
{
    private const int MaxReconnectAttempts = 100;
    private const int MaxBackoffDelaySeconds = 30;
    private const int BackoffBase = 2;

    private IConnection? Connection { get; set; }

    private readonly ConnectionFactory _connectionFactory = new() { HostName = "localhost" };
    private readonly SemaphoreSlim _semaphore = new(1, 1);
    private readonly CancellationTokenSource _disposeCts = new();
    private readonly ILogger<RabbitMqConnectionProvider> _logger;

    public RabbitMqConnectionProvider(
        ILogger<RabbitMqConnectionProvider> logger)
    {
        _logger = logger;

        _logger.LogInformation("RabbitMQ connection factory initialized for host {Host}:{Port}",
            _connectionFactory.HostName, _connectionFactory.Port);
    }

    public async Task<IChannel?> CreateChannelAsync(
        CancellationToken cancellationToken = default)
    {
        if (Connection is null || !Connection.IsOpen) _logger.LogDebug("Creating RabbitMQ connection...");

        return Connection is null ? null : await Connection.CreateChannelAsync(cancellationToken: cancellationToken);
    }

    public async Task EnsureConnected(
        CancellationToken cancellationToken = default)
    {
        if (Connection is not null && Connection.IsOpen) return;

        await Reconnect(cancellationToken);

        if (Connection is null || !Connection.IsOpen)
            throw new InvalidOperationException("RabbitMQ connection is not available after reconnection attempts");
    }

    private async Task ConnectInternal(
        CancellationToken cancellationToken)
    {
        using var linkedCts = CancellationTokenSource.CreateLinkedTokenSource(cancellationToken, _disposeCts.Token);

        await _semaphore.WaitAsync(linkedCts.Token);

        try
        {
            if (Connection is not null && Connection.IsOpen) return;

            _logger.LogDebug("Creating new RabbitMQ connection...");

            var connection = await _connectionFactory.CreateConnectionAsync(linkedCts.Token);
            connection.ConnectionShutdownAsync += OnConnectionShutdown;

            Connection = connection;

            _logger.LogInformation("RabbitMQ connection established successfully");
        }
        catch (OperationCanceledException) when (_disposeCts.IsCancellationRequested)
        {
            _logger.LogDebug("Connection creation cancelled due to application shutdown");
            throw;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Failed to create RabbitMQ connection");
            throw;
        }
        finally
        {
            _semaphore.Release();
        }
    }

    private async Task Reconnect(
        CancellationToken cancellationToken)
    {
        await CleanUp(cancellationToken);

        for (var attempt = 1; attempt <= MaxReconnectAttempts; attempt++)
        {
            if (cancellationToken.IsCancellationRequested) break;

            try
            {
                await ConnectInternal(cancellationToken);
                return;
            }
            catch (OperationCanceledException) when (cancellationToken.IsCancellationRequested)
            {
                return;
            }
            catch (Exception ex)
            {
                var delay = Math.Min((int) Math.Pow(BackoffBase, attempt), MaxBackoffDelaySeconds);

                _logger.LogWarning(ex,
                    "RabbitMQ connection attempt {Attempt}/{MaxAttempts} failed. Next retry in {Delay}s", attempt,
                    MaxReconnectAttempts, delay);

                await Task.Delay(TimeSpan.FromSeconds(delay), cancellationToken);
            }
        }
    }

    private async Task OnConnectionShutdown(
        object sender,
        ShutdownEventArgs ev)
    {
        _logger.LogWarning("RabbitMQ connection shut down. Reason: {Reason}, Initiator: {Initiator}", ev.ReplyText,
            ev.Initiator);

        await Reconnect(_disposeCts.Token);
    }

    private async Task CleanUp(
        CancellationToken cancellationToken = default)
    {
        try
        {
            if (Connection is { IsOpen: true })
            {
                _logger.LogDebug("Closing RabbitMQ connection gracefully...");
                await Connection.CloseAsync(cancellationToken: cancellationToken);
            }
        }
        catch (IOException ex)
        {
            _logger.LogWarning(ex, "IOException during connection cleanup (expected if connection is dead)");
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Unexpected error during RabbitMQ connection cleanup");
        }
        finally
        {
            if (Connection is not null)
            {
                Connection.ConnectionShutdownAsync -= OnConnectionShutdown;
            }

            Connection = null;
        }
    }

    public async ValueTask DisposeAsync()
    {
        _logger.LogInformation("Disposing RabbitMQ connection factory...");

        await _disposeCts.CancelAsync();
        await CleanUp();
        _semaphore.Dispose();
        _disposeCts.Dispose();

        _logger.LogInformation("RabbitMQ connection factory disposed");
    }
}
