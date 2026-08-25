using System.Text;
using System.Text.Json;
using Microsoft.Extensions.Logging;
using RabbitMQ.Client;
using RabbitMQ.Client.Events;
using RabbitMq.Library.Broker.Connection;
using RabbitMq.Library.Broker.IntegrationEvents;

namespace RabbitMq.Library.Broker.Receive;

public sealed class RabbitMqMessageReceiver(
    RabbitMqChannelFactory channelFactory,
    ILogger<RabbitMqMessageReceiver> logger,
    ReceiveDispatcher dispatcher)
{
    private static readonly JsonSerializerOptions JsonOptions =
        new() { PropertyNamingPolicy = JsonNamingPolicy.CamelCase };

    public async Task ReceiveMessageAsync(
        IntegrationEventDescriptor descriptor,
        CancellationToken cancellationToken = default)
    {
        var channel = await channelFactory.CreateChannelAsync(cancellationToken);
        if (channel is null || !channel.IsOpen)
        {
            throw new InvalidOperationException("RabbitMQ channel is not available.");
        }

        try
        {
            await channel.BasicQosAsync(0, 10, false, cancellationToken);
            var consumer = new AsyncEventingBasicConsumer(channel);
            consumer.ReceivedAsync += async (
                _,
                ea) =>
            {
                try
                {
                    var body = ea.Body.ToArray();
                    var json = Encoding.UTF8.GetString(body);
                    var message = JsonSerializer.Deserialize(json, descriptor.EventType, JsonOptions);
                    if (message is null)
                    {
                        throw new InvalidOperationException($"Failed to deserialize '{descriptor.EventType.Name}'.");
                    }

                    logger.LogInformation("Received {EventType} from queue {QueueName}", descriptor.EventType.Name,
                        descriptor.QueueName);
                    await dispatcher.DispatchAsync(descriptor.EventType, message, cancellationToken);
                    logger.LogInformation("Successfully processed {EventType} from queue {QueueName}",
                        descriptor.EventType.Name, descriptor.QueueName);
                    await channel.BasicAckAsync(ea.DeliveryTag, false, cancellationToken);
                }
                catch (Exception ex)
                {
                    logger.LogError(ex, "Failed to process message from queue {QueueName}", descriptor.QueueName);
                    await channel.BasicNackAsync(ea.DeliveryTag, false, false, cancellationToken);
                }
            };
            await channel.BasicConsumeAsync(descriptor.QueueName, false, consumer, cancellationToken);
            logger.LogInformation("Consumer started for queue {QueueName}", descriptor.QueueName);
            await WaitUntilChannelClosedAsync(channel, cancellationToken);
        }
        finally
        {
            await channel.DisposeAsync();
        }
    }

    private static Task WaitUntilChannelClosedAsync(
        IChannel channel,
        CancellationToken cancellationToken)
    {
        var tcs = new TaskCompletionSource<bool>(TaskCreationOptions.RunContinuationsAsynchronously);
        using var registration = cancellationToken.Register(() => tcs.TrySetResult(true));
        channel.ChannelShutdownAsync += (
            _,
            _) =>
        {
            tcs.TrySetResult(true);
            return Task.CompletedTask;
        };
        return tcs.Task;
    }
}
