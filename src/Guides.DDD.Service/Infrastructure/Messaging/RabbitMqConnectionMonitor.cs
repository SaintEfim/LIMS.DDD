namespace Guides.DDD.Service.Infrastructure.Messaging;

public sealed class RabbitMqConnectionMonitor(
    RabbitMqConnectionProvider connectionProvider,
    RabbitMqTopologyDeclarator rabbitMqTopologyDeclarator,
    ILogger<RabbitMqConnectionMonitor> logger) : BackgroundService
{
    protected override async Task ExecuteAsync(
        CancellationToken stoppingToken)
    {
        while (!stoppingToken.IsCancellationRequested)
        {
            try
            {
                await connectionProvider.EnsureConnectedAsync(stoppingToken);

                await rabbitMqTopologyDeclarator.DeclareAllAsync(stoppingToken);

                await Task.Delay(TimeSpan.FromSeconds(30), stoppingToken);
            }
            catch (OperationCanceledException) when (stoppingToken.IsCancellationRequested)
            {
                break;
            }
            catch (Exception ex)
            {
                logger.LogError(ex, "Failed to maintain RabbitMQ connection");
                await Task.Delay(TimeSpan.FromSeconds(5), stoppingToken);
            }
        }
    }
}
