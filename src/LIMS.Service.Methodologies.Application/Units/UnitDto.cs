using LIMS.Service.Methodologies.Domain.UnitSnapshots;

namespace LIMS.Service.Methodologies.Application.Units;

public sealed record UnitDto(Guid Id, string Name)
{
    public static UnitDto FromSnapshot(
        UnitSnapshot snapshot)
    {
        return new UnitDto(snapshot.Id.Value, snapshot.Name.Value);
    }
}
