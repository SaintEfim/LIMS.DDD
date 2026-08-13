namespace Guides.DDD.Service.Infrastructure.Messaging;

public class RabbitMqConnectionBackgroundService(RabbitMqChannelManager provider) : BackgroundService
{
    protected override async Task ExecuteAsync(
        CancellationToken stoppingToken)
    {
        await provider.EnsureConnected(stoppingToken);
    }
}
