using LIMS.Service.Methodologies.Domain.SeedWork;
using LIMS.Service.Methodologies.Domain.SeedWork.Result;
using LIMS.Service.Methodologies.Domain.SeedWork.ValueObjects;

namespace LIMS.Service.Methodologies.Domain.UnitSnapshots;

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

    public static Result<UnitSnapshot, Exception> Create(
        UnitId id,
        Name name)
    {
        var unit = new UnitSnapshot
        {
            Id = id,
            Name = name
        };

        return Result<UnitSnapshot, Exception>.Success(unit);
    }
}
