using LIMS.DDD.Service.Domain.SeedWork.Result;
using LIMS.DDD.Service.Domain.SeedWork.ValueObjects;

namespace LIMS.DDD.Service.Domain.SeedWork.Snapshots;

public sealed class UnitSnapshot : SoftDeletableModel
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

    public void Update(
        Name name) =>
        Name = name;

    public void Delete()
    {
        IsDeleted = true;
        DeletedAt = DateTimeOffset.UtcNow;
    }
}
