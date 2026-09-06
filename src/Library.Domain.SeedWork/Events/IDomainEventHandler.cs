namespace Library.Domain.SeedWork.Events;

public interface IDomainEventHandler<in TEvent>
    where TEvent : class, IDomainEvent
{
    Task Handle(
        TEvent @event,
        CancellationToken cancellationToken = default);
}
