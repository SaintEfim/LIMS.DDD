using RabbitMq.Library.Outbox;

namespace LIMS.Service.Methodologies.Persistence.Repositories;

public class OutboxRepository(ApplicationDbContext context) : IOutboxRepository
{
    public void InsertOutboxMessage<TMessage>(
        TMessage message)
        where TMessage : notnull
    {
        context.InsertOutboxMessage(message);
    }
}
