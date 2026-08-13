using RabbitMQ.Client;
using RabbitMQ.Client.Events;

namespace Guides.DDD.Service.Infrastructure.Messaging;

public sealed class RabbitMqChannelManager : IAsyncDisposable
{
    private const int MaxReconnectAttempts = 10;
    private const int MaxBackoffDelaySeconds = 30;
    private const int BackoffBase = 2;

    private IConnection? _connection;

    private readonly ConnectionFactory _connectionFactory = new() { HostName = "localhost" };
    private readonly SemaphoreSlim _semaphore = new(1, 1);
    private readonly CancellationTokenSource _disposeCts = new();
    private readonly ILogger<RabbitMqChannelManager> _logger;

    public RabbitMqChannelManager(
        ILogger<RabbitMqChannelManager> logger)
    {
        _logger = logger;

        _logger.LogInformation("RabbitMQ connection factory initialized for host {Host}:{Port}",
            _connectionFactory.HostName, _connectionFactory.Port);
    }

    public async Task<IChannel?> CreateChannelAsync(
        CancellationToken cancellationToken = default)
    {
        if (_connection is not null && _connection.IsOpen)
        {
            return _connection is null
                ? null
                : await _connection.CreateChannelAsync(cancellationToken: cancellationToken);
        }

        _logger.LogDebug("RabbitMQ connection is currently unavailable.");

        return null;
    }

    // for background service
    public async Task EnsureConnected(
        CancellationToken cancellationToken = default)
    {
        if (_connection is not null && _connection.IsOpen) return;

        await Reconnect(cancellationToken);

        if (_connection is null || !_connection.IsOpen)
            throw new InvalidOperationException("RabbitMQ connection is not available after reconnection attempts");
    }

    private async Task ConnectInternal(
        CancellationToken cancellationToken)
    {
        using var linkedCts = CancellationTokenSource.CreateLinkedTokenSource(cancellationToken, _disposeCts.Token);

        await _semaphore.WaitAsync(linkedCts.Token);

        try
        {
            if (_connection is not null && _connection.IsOpen) return;

            _logger.LogDebug("Creating new RabbitMQ connection...");

            var connection = await _connectionFactory.CreateConnectionAsync(linkedCts.Token);
            connection.ConnectionShutdownAsync += OnConnectionShutdown;

            _connection = connection;

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
            if (_connection is { IsOpen: true })
            {
                _logger.LogDebug("Closing RabbitMQ connection gracefully...");
                await _connection.CloseAsync(cancellationToken: cancellationToken);
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
            if (_connection is not null)
            {
                _connection.ConnectionShutdownAsync -= OnConnectionShutdown;
            }

            _connection = null;
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
