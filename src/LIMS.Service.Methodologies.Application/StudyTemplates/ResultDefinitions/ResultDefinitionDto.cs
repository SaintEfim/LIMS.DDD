using LIMS.Service.Methodologies.Application.Units;
using LIMS.Service.Methodologies.Domain.StudyTemplateAggregate.Entities.ResultDefinitions;
using LIMS.Service.Methodologies.Domain.UnitSnapshots;

namespace LIMS.Service.Methodologies.Application.StudyTemplates.ResultDefinitions;

public sealed record ResultDefinitionDto(
    Guid Id,
    UnitSnapshotDto Unit,
    string ResultInstance,
    double? MinValue,
    double? MaxValue)
{
    public static ResultDefinitionDto FromDomain(
        UnitSnapshot unit,
        ResultDefinition resultDefinition)
    {
        return new ResultDefinitionDto(resultDefinition.Id.Value, UnitSnapshotDto.FromSnapshot(unit),
            resultDefinition.ResultInstance, resultDefinition.Specification.MinValue,
            resultDefinition.Specification.MaxValue);
    }
}
