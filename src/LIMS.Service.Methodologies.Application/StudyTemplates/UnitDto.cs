using LIMS.Service.Methodologies.Domain.UnitSnapshots;

namespace LIMS.Service.Methodologies.Application.StudyTemplates;

public sealed record UnitDto(Guid Id, string Name)
{
    public static UnitDto? FromSnapshot(
        UnitSnapshot? snapshot)
    {
        return snapshot is null ? null : new UnitDto(snapshot.Id.Value, snapshot.Name.Value);
    }
}
