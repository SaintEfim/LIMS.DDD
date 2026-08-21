using LIMS.Service.LaboratoryOperations.Domain.UnitSnapshots;

namespace LIMS.Service.LaboratoryOperations.Application.Units;

public sealed record UnitSnapshotDto(Guid Id, string Name)
{
    public static UnitSnapshotDto FromSnapshot(
        UnitSnapshot snapshot)
    {
        return new UnitSnapshotDto(snapshot.Id.Value, snapshot.Name.Value);
    }
}
