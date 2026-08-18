using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;

namespace RabbitMq.Library.QuickStart.Connection;

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
                var reconnected = await connectionProvider.EnsureConnectedAsync(stoppingToken);

                if (reconnected)
                {
                    await rabbitMqTopologyDeclarator.DeclareAsync(stoppingToken);
                }

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
