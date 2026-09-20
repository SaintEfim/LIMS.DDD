using LIMS.Service.LaboratoryOperations.Application.BackgroundOperations;

namespace LIMS.Service.LaboratoryOperations.API.BackgroundServices;

public class NotificationBackgroundService(
    IServiceScopeFactory scopeFactory,
    ILogger<NotificationBackgroundService> logger) : BackgroundService
{
    private const int ChunkSize = 5;
    private static readonly TimeSpan FallbackInterval = TimeSpan.FromSeconds(5);

    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        logger.LogInformation("Notification background service started.");

        try
        {
            while (!stoppingToken.IsCancellationRequested)
            {
                IReadOnlyList<BackgroundOperationDto> operations;

                using (var scope = scopeFactory.CreateScope())
                {
                    var services = scope.ServiceProvider.GetRequiredService<BackgroundOperationServices>();
                    operations = await services.Queries.GetPendingAsync(ChunkSize, stoppingToken);
                }

                if (operations.Count == 0)
                {
                    await Task.Delay(FallbackInterval, stoppingToken);
                    continue;
                }

                await Task.WhenAll(operations.Select(operation => ProcessAsync(operation, stoppingToken)));
            }
        }
        catch (OperationCanceledException) when (stoppingToken.IsCancellationRequested)
        {
            // Normal shutdown.
        }
        catch (Exception exception)
        {
            logger.LogCritical(exception, "Notification background service terminated unexpectedly.");
        }
        finally
        {
            logger.LogInformation("Notification background service stopped.");
        }
    }

    private async Task ProcessAsync(BackgroundOperationDto operation, CancellationToken cancellationToken)
    {
        try
        {
            using var scope = scopeFactory.CreateScope();
            var processor = scope.ServiceProvider.GetRequiredKeyedService<IProcessor>(operation.Type);
            await processor.ExecuteAsync(operation, cancellationToken);
        }
        catch (Exception exception) when (exception is not OperationCanceledException || !cancellationToken.IsCancellationRequested)
        {
            logger.LogError(exception, "Failed to process background operation {OperationId}.", operation.Id);
        }
    }
}
