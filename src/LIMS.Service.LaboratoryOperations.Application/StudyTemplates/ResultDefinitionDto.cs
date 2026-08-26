using LIMS.Service.LaboratoryOperations.Application.Units;
using LIMS.Service.LaboratoryOperations.Domain.StudyTemplateSnapshots.ResultDefinitions;
using LIMS.Service.LaboratoryOperations.Domain.UnitSnapshots;

namespace LIMS.Service.LaboratoryOperations.Application.StudyTemplates;

public sealed record ResultDefinitionDto(
    Guid Id,
    string ResultInstance,
    UnitSnapshotDto? Unit,
    double? SpecMin,
    double? SpecMax)
{
    public static ResultDefinitionDto FromSnapshot(
        UnitSnapshot? unitSnapshot,
        ResultDefinitionSnapshot snapshot)
    {
        return new ResultDefinitionDto(snapshot.Id.Value, snapshot.ResultInstance,
            unitSnapshot is null ? null : UnitSnapshotDto.FromSnapshot(unitSnapshot), snapshot.Specification.MinValue,
            snapshot.Specification.MaxValue);
    }
}
