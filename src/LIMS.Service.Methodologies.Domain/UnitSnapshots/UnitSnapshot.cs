using Domain.SeedWork.SeedWork;
using Domain.SeedWork.SeedWork.ValueObjects;

namespace LIMS.Service.Methodologies.Domain.UnitSnapshots;

public sealed class UnitSnapshot
    : SoftDeletableModel,
        IAggregateRoot
{
    private UnitSnapshot()
    {
    }

    public UnitSnapshot(
        UnitId id,
        Name name)
    {
        Id = id;
        Name = name;
    }

    // link for original unit id from guid service
    public UnitId Id { get; init; }

    public Name Name { get; set; } = null!;
}
