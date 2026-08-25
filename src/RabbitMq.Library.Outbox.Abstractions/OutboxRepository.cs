namespace RabbitMq.Library.Outbox;

public interface IOutboxRepository
{
    void InsertOutboxMessage<T>(
        T message)
        where T : notnull;
}
