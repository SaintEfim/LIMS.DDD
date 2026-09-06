using Library.Domain.SeedWork;
using Library.Domain.SeedWork.ValueObjects;

namespace LIMS.Service.LaboratoryOperations.Domain.UnitSnapshots;

public sealed class UnitSnapshot : IAggregateRoot
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
