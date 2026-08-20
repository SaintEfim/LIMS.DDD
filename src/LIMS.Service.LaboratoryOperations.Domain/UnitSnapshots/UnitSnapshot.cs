using Domain.SeedWork.SeedWork;
using Domain.SeedWork.SeedWork.SoftDeletable;
using Domain.SeedWork.SeedWork.ValueObjects;

namespace LIMS.Service.LaboratoryOperations.Domain.UnitSnapshots;

public sealed class UnitSnapshot
    : SoftDeletableModel,
        IAggregateRoot
{
    // link for original unit id from guid service
    public UnitId Id { get; init; }

    public Name Name { get; set; } = null!;

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
}
