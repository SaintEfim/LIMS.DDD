using LIMS.Service.LaboratoryOperations.Domain.UnitSnapshots;

namespace LIMS.Service.LaboratoryOperations.Application.Units;

public sealed record UnitDto(Guid Id, string Name)
{
    public static UnitDto FromSnapshot(
        UnitSnapshot snapshot)
    {
        return new UnitDto(snapshot.Id.Value, snapshot.Name.Value);
    }
}
