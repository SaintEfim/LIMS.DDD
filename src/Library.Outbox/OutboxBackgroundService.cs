using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;

namespace Library.Outbox;

public sealed class OutboxBackgroundService(
    IServiceScopeFactory scopeFactory,
    ILogger<OutboxBackgroundService> logger) : BackgroundService
{
    protected override async Task ExecuteAsync(
        CancellationToken stoppingToken)
    {
        logger.LogInformation("Outbox background service started.");

        try
        {
            while (!stoppingToken.IsCancellationRequested)
            {
                await Task.Delay(OutboxConstants.FallbackInterval, stoppingToken);

                while (!stoppingToken.IsCancellationRequested)
                {
                    using var scope = scopeFactory.CreateScope();
                    var processor = scope.ServiceProvider.GetRequiredService<OutboxProcessor>();
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
}
