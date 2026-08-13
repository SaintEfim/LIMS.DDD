namespace Guides.DDD.Service.Infrastructure.Messaging;

public class RabbitMqConnectionBackgroundService(RabbitMqConnectionProvider provider) : BackgroundService
{
    protected override async Task ExecuteAsync(
        CancellationToken stoppingToken)
    {
        while (!stoppingToken.IsCancellationRequested)
        {
            try
            {
                await provider.EnsureConnected(stoppingToken);
                break;
            }
            catch
            {
                await Task.Delay(5000, stoppingToken);
            }
        }
    }
}
