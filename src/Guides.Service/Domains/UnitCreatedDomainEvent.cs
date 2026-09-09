using Library.Domain.SeedWork.Events;

namespace Guides.Service.Domains;

public sealed record UnitCreatedDomainEvent(Guid UnitId) : IDomainEvent;
