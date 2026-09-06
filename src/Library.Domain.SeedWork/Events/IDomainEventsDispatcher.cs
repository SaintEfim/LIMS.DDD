namespace Library.Domain.SeedWork.Events;

public interface IDomainEventsDispatcher
{
    Task DispatchAsync(
        IEnumerable<IDomainEvent> domainEvents,
        CancellationToken cancellationToken = default);
}
