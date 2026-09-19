using Library.Domain.SeedWork.Events;

namespace Service.Guides.Domain;

public sealed record UnitCreatedDomainEvent(Unit Unit) : IDomainEvent;
