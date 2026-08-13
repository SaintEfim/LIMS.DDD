namespace Guides.DDD.Service.Infrastructure.Messaging;

public class RabbitMqConnectionBackgroundService(RabbitMqConnectionService connectionService) : BackgroundService
{
    protected override async Task ExecuteAsync(
        CancellationToken stoppingToken)
    {
        await connectionService.EnsureConnected(stoppingToken);
    }
}
