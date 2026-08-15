using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;

namespace RabbitMq.Library.QuickStart.Receive;

public class ReceiveHandlersBackgroundService(
    IReadOnlyDictionary<Type, IntegrationEventDescriptor> events,
    RabbitMqMessageReceiver rabbitMqMessageReceiver,
    ILogger<ReceiveHandlersBackgroundService> logger) : BackgroundService
{
    protected override async Task ExecuteAsync(
        CancellationToken stoppingToken)
    {
        var tasks = events.Values
            .Select(descriptor => RunConsumerAsync(descriptor, stoppingToken))
            .ToArray();

        await Task.WhenAll(tasks);
    }

    private async Task RunConsumerAsync(
        IntegrationEventDescriptor descriptor,
        CancellationToken cancellationToken)
    {
        while (!cancellationToken.IsCancellationRequested)
        {
            try
            {
                await rabbitMqMessageReceiver.ReceiveMessageAsync(descriptor, cancellationToken);
            }
            catch (OperationCanceledException) when (cancellationToken.IsCancellationRequested)
            {
                break;
            }
            catch (Exception ex)
            {
                logger.LogError(ex, "Consumer for queue {QueueName} failed. Restarting...", descriptor.QueueName);
            }

            if (!cancellationToken.IsCancellationRequested)
            {
                await Task.Delay(TimeSpan.FromSeconds(5), cancellationToken);
            }
        }
    }
}
