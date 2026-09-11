using Library.Domain.SeedWork;
using Library.Domain.SeedWork.ValueObjects;

namespace Guides.Service.Domain;

public class Unit
    : ISoftDeletable,
        IAggregateRoot
{
    public Unit(
        Name name)
    {
        Id = new UnitId(Guid.NewGuid());
        Name = name;
    }

    public UnitId Id { get; set; }
    public Name Name { get; set; }
    public bool IsDeleted { get; private set; }
    public DateTimeOffset? DeletedAt { get; private set; }

    public void MarkAsDeleted()
    {
        IsDeleted = true;
        DeletedAt = DateTimeOffset.UtcNow;
    }
}
