using LIMS.Service.LaboratoryOperations.Application.BackgroundOperations;

namespace LIMS.Service.LaboratoryOperations.API.BackgroundServices;

public sealed class BackgroundOperationWorker(
    IServiceScopeFactory scopeFactory,
    ILogger<BackgroundOperationWorker> logger) : BackgroundService
{
    private const int ChunkSize = 5;
    private static readonly TimeSpan FallbackInterval = TimeSpan.FromSeconds(5);

    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        logger.LogInformation("Background operation worker started.");

        try
        {
            while (!stoppingToken.IsCancellationRequested)
            {
                IReadOnlyList<BackgroundOperationDto> operations;

                await using (var scope = scopeFactory.CreateAsyncScope())
                {
                    var queries = scope.ServiceProvider.GetRequiredService<BackgroundOperationQueries>();
                    operations = await queries.GetPendingAsync(ChunkSize, stoppingToken);
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
            logger.LogCritical(exception, "Background operation worker terminated unexpectedly.");
        }
        finally
        {
            logger.LogInformation("Background operation worker stopped.");
        }
    }

    private async Task ProcessAsync(BackgroundOperationDto operation, CancellationToken cancellationToken)
    {
        try
        {
            await using var scope = scopeFactory.CreateAsyncScope();
            var processor = scope.ServiceProvider.GetRequiredKeyedService<IBackgroundOperationProcessor>(operation.Type);
            await processor.ExecuteAsync(operation, cancellationToken);
        }
        catch (Exception exception) when (exception is not OperationCanceledException || !cancellationToken.IsCancellationRequested)
        {
            logger.LogError(exception, "Failed to process background operation {OperationId}.", operation.Id);
        }
    }
}
