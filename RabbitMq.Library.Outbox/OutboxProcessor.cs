using System.Reflection;
using System.Text.Json;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using RabbitMq.Library.Broker;

namespace RabbitMq.Library.Outbox;

public sealed class OutboxProcessor<TDbContext>(
    TDbContext context,
    IMessageBus messageBus,
    ILogger<OutboxProcessor<TDbContext>> logger)
    where TDbContext : DbContext
{
    private const int BatchSize = 10;

    public async Task<bool> Execute(
        CancellationToken cancellationToken = default)
    {
        var outboxMessages = await context.Set<OutboxMessage>()
            .Where(x => x.ProcessedOnUtc == null)
            .OrderBy(x => x.OccurredOnUtc)
            .Take(BatchSize)
            .ToListAsync(cancellationToken);

        if (outboxMessages.Count == 0)
        {
            return false;
        }

        var sendMethod = typeof(IMessageBus).GetMethod(nameof(IMessageBus.SendAsync));

        if (sendMethod is null)
        {
            throw new InvalidOperationException(
                $"{nameof(IMessageBus)}.{nameof(IMessageBus.SendAsync)} method was not found.");
        }

        foreach (var message in outboxMessages)
        {
            cancellationToken.ThrowIfCancellationRequested();

            try
            {
                var messageType = Type.GetType(message.Type);

                if (messageType is null)
                {
                    MarkAsFailed(message, $"Message type '{message.Type}' was not found.");

                    continue;
                }

                if (!typeof(IIntegrationEvent).IsAssignableFrom(messageType))
                {
                    MarkAsFailed(message, $"Type '{message.Type}' does not implement {nameof(IIntegrationEvent)}.");

                    continue;
                }

                var deserializedMessage = JsonSerializer.Deserialize(message.Content, messageType);

                if (deserializedMessage is null)
                {
                    MarkAsFailed(message, "Failed to deserialize outbox message.");

                    continue;
                }

                var genericSendMethod = sendMethod.MakeGenericMethod(messageType);

                await (Task) genericSendMethod.Invoke(messageBus, [deserializedMessage, cancellationToken])!;

                message.ProcessedOnUtc = DateTime.UtcNow;
                message.Error = null;

                logger.LogDebug("Outbox message {OutboxMessageId} was published successfully.", message.Id);
            }
            catch (OperationCanceledException) when (cancellationToken.IsCancellationRequested)
            {
                throw;
            }
            catch (TargetInvocationException exception) when (exception.InnerException is not null)
            {
                HandleProcessingException(message, exception.InnerException);

                await context.SaveChangesAsync(cancellationToken);

                return false;
            }
            catch (Exception exception)
            {
                HandleProcessingException(message, exception);

                await context.SaveChangesAsync(cancellationToken);

                return false;
            }
        }

        await context.SaveChangesAsync(cancellationToken);

        return true;
    }

    private void HandleProcessingException(
        OutboxMessage message,
        Exception exception)
    {
        message.Error = exception.Message;

        logger.LogError(exception,
            "Failed to publish outbox message {OutboxMessageId} of type {MessageType}. " +
            "The message will be retried later.", message.Id, message.Type);
    }

    private static void MarkAsFailed(
        OutboxMessage message,
        string error)
    {
        message.Error = error;
        message.ProcessedOnUtc = DateTime.UtcNow;
    }
}
