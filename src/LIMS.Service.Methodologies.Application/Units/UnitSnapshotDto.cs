using LIMS.Service.Methodologies.Domain.UnitSnapshots;

namespace LIMS.Service.Methodologies.Application.Units;

public sealed record UnitSnapshotDto(Guid Id, string Name)
{
    public static UnitSnapshotDto FromSnapshot(
        UnitSnapshot snapshot)
    {
        return new UnitSnapshotDto(snapshot.Id.Value, snapshot.Name.Value);
    }
}
