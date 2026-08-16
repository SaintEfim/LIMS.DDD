using LIMS.DDD.Service.Domain.SeedWork.Snapshots;
using LIMS.DDD.Service.Domain.StudyTemplateContext.StudyTemplateAggregate.Entities.ResultDefinitions;

namespace LIMS.DDD.Service.Application.StudyTemplates.ResultDefinitions;

public sealed record ResultDefinitionDto(
    Guid Id,
    UnitDto Unit,
    string ResultInstance,
    double? MinValue,
    double? MaxValue)
{
    public static ResultDefinitionDto FromDomain(
        UnitSnapshot? unit,
        ResultDefinition resultDefinition)
    {
        return new ResultDefinitionDto(resultDefinition.Id.Value, UnitDto.FromSnapshot(unit),
            resultDefinition.ResultInstance, resultDefinition.Specification.MinValue,
            resultDefinition.Specification.MaxValue);
    }
}
