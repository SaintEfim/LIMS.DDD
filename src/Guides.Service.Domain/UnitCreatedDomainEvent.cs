using Library.Domain.SeedWork.Events;

namespace Guides.Service.Domain;

public sealed record UnitCreatedDomainEvent(Unit Unit) : IDomainEvent;
