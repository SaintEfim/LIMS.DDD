using Guides.Service.Domain;
using Library.Broker.Messages;
using Library.Domain.SeedWork.Events;
using Library.Outbox;

namespace Guides.Service.Application.Commands;

public class UnitCreatedDomainEventHandler(IOutboxRepository outboxRepository)
    : IDomainEventHandler<UnitCreatedDomainEvent>
{
    public Task Handle(
        UnitCreatedDomainEvent @event,
        CancellationToken cancellationToken = default)
    {
        var unit = @event.Unit;

        outboxRepository.InsertOutboxMessage(new UnitCreatedMessage(unit.Id.Value, unit.Name.Value));

        return Task.CompletedTask;
    }
}
