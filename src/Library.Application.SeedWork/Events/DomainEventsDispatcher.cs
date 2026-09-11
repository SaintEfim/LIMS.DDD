using Library.Domain.SeedWork.Events;

namespace Library.Application.SeedWork.Events;

internal sealed class DomainEventsDispatcher(IServiceProvider serviceProvider) : IDomainEventsDispatcher
{
    public async Task DispatchAsync(
        IEnumerable<IDomainEvent> domainEvents,
        CancellationToken cancellationToken = default)
    {
        foreach (var @event in domainEvents)
        {
            var eventType = @event.GetType();
            var handlerType = typeof(IDomainEventHandler<>).MakeGenericType(eventType);

            var handler = serviceProvider.GetService(handlerType);
            if (handler == null) continue;

            var handleMethod = handlerType.GetMethod(nameof(IDomainEventHandler<>.Handle));
            var task = (Task) handleMethod!.Invoke(handler, [@event, cancellationToken])!;

            await task.ConfigureAwait(false);
        }
    }
}
