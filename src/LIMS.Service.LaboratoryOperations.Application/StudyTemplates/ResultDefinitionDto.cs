using LIMS.Service.LaboratoryOperations.Domain.StudyTemplateSnapshots.ResultDefinitions;

namespace LIMS.Service.LaboratoryOperations.Application.StudyTemplates;

public sealed record ResultDefinitionDto(Guid Id, string ResultInstance, Guid UnitId, double? SpecMin, double? SpecMax)
{
    public static ResultDefinitionDto FromSnapshot(
        ResultDefinitionSnapshot snapshot)
    {
        return new ResultDefinitionDto(snapshot.Id.Value, snapshot.ResultInstance, snapshot.UnitId.Value,
            snapshot.Specification.MinValue, snapshot.Specification.MaxValue);
    }
}
