using Library.Domain.SeedWork;
using Library.Domain.SeedWork.ValueObjects;

namespace Guides.Service.Domain;

public class Unit
    : EntityBase,
        ISoftDeletable,
        IAggregateRoot
{
    private Unit()
    {
        // Required by EF Core.
    }

    public Unit(
        Name name)
        : this(new UnitId(Guid.NewGuid()), name)
    {
    }

    public static Unit CreateSystem(
        UnitId id,
        Name name)
    {
        return new Unit(id, name);
    }

    private Unit(
        UnitId id,
        Name name)
    {
        Id = id;
        Name = name;

        AddDomainEvent(new UnitCreatedDomainEvent(this));
    }

    public UnitId Id { get; private set; }
    public Name Name { get; private set; } = null!;
    public bool IsDeleted { get; private set; }
    public DateTimeOffset? DeletedAt { get; private set; }

    public void MarkAsDeleted()
    {
        IsDeleted = true;
        DeletedAt = DateTimeOffset.UtcNow;
    }
}
