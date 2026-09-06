using Guides.Service.Domains;
using Library.Domain.SeedWork.Events;

namespace Guides.Service.Commands;

public class UnitCreatedDomainEventHandler : IDomainEventHandler<UnitCreatedDomainEvent>
{
    public Task Handle(
        UnitCreatedDomainEvent @event,
        CancellationToken cancellationToken = default)
    {
        throw new NotImplementedException();
    }
}
