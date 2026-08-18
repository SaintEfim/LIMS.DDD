using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;

namespace RabbitMq.Library.QuickStart.Receive;

public sealed class ReceiveHandlersBackgroundService(
    IReadOnlyDictionary<Type, IntegrationEventDescriptor> consumedEvents,
    RabbitMqMessageReceiver rabbitMqMessageReceiver,
    ILogger<ReceiveHandlersBackgroundService> logger) : BackgroundService
{
    protected override async Task ExecuteAsync(
        CancellationToken stoppingToken)
    {
        if (consumedEvents.Count == 0)
        {
            logger.LogInformation("No message handlers registered.");
            return;
        }

        var tasks = consumedEvents.Values
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
