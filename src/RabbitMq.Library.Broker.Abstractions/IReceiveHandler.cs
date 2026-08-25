namespace RabbitMq.Library.Broker;

public interface IReceiveHandler<in TMessage>
    where TMessage : IIntegrationEvent
{
    Task HandleAsync(
        TMessage message,
        CancellationToken cancellationToken = default);
}
