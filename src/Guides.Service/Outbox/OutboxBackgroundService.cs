using Microsoft.EntityFrameworkCore;

namespace Guides.Service.Outbox;

public sealed class OutboxBackgroundService<TDbContext>(
    IServiceScopeFactory scopeFactory,
    OutboxSignal signal,
    ILogger<OutboxBackgroundService<TDbContext>> logger) : BackgroundService
    where TDbContext : DbContext
{
    protected override async Task ExecuteAsync(
        CancellationToken stoppingToken)
    {
        logger.LogInformation("Outbox background service started.");

        try
        {
            while (!stoppingToken.IsCancellationRequested)
            {
                await WaitForWorkAsync(stoppingToken);

                while (!stoppingToken.IsCancellationRequested)
                {
                    await using var scope = scopeFactory.CreateAsyncScope();

                    var processor = scope.ServiceProvider.GetRequiredService<OutboxProcessor<TDbContext>>();

                    var hasMoreWork = await processor.Execute(stoppingToken);

                    if (!hasMoreWork)
                    {
                        break;
                    }
                }
            }
        }
        catch (OperationCanceledException) when (stoppingToken.IsCancellationRequested)
        {
            // Normal shutdown.
        }
        catch (Exception exception)
        {
            logger.LogCritical(exception, "Outbox background service terminated unexpectedly.");
        }
        finally
        {
            logger.LogInformation("Outbox background service stopped.");
        }
    }

    private async Task WaitForWorkAsync(
        CancellationToken cancellationToken)
    {
        var signalTask = signal.WaitAsync(cancellationToken)
            .AsTask();

        var fallbackTask = Task.Delay(Constants.FallbackInterval, cancellationToken);

        await Task.WhenAny(signalTask, fallbackTask);
    }
}
