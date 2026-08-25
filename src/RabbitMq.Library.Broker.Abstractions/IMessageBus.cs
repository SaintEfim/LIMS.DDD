namespace RabbitMq.Library.Broker;

public interface IMessageBus
{
    Task SendAsync<T>(
        T message,
        CancellationToken cancellationToken = default)
        where T : IIntegrationEvent;
}
