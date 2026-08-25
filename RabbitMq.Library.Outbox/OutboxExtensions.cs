using System.Text.Json;
using Microsoft.EntityFrameworkCore;

namespace RabbitMq.Library.Outbox;

public static class OutboxExtensions
{
    public static void InsertOutboxMessage<T>(
        this DbContext context,
        T message)
        where T : notnull
    {
        var outboxMessage = new OutboxMessage
        {
            Id = Guid.NewGuid(),
            Type = $"{message.GetType().FullName}, {message.GetType().Assembly.GetName().Name}",
            Content = JsonSerializer.Serialize(message),
            OccurredOnUtc = DateTime.UtcNow
        };

        context.Set<OutboxMessage>()
            .Add(outboxMessage);
    }
}
