using Microsoft.EntityFrameworkCore;

namespace Library.Outbox;

public class OutboxRepository(DbContext context) : IOutboxRepository
{
    public void InsertOutboxMessage<TMessage>(
        TMessage message)
        where TMessage : notnull
    {
        context.InsertOutboxMessage(message);
    }
}
