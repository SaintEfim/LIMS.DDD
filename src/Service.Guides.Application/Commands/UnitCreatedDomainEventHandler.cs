using Library.Broker.Messages;
using Library.Domain.SeedWork.Events;
using Library.Outbox;
using Service.Guides.Domain;

namespace Service.Guides.Application.Commands;

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
