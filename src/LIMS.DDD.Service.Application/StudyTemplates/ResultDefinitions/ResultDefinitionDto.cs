using LIMS.DDD.Service.Domain.StudyTemplateContext.StudyTemplateAggregate.Entities.ResultDefinitions;

namespace LIMS.DDD.Service.Application.StudyTemplates.ResultDefinitions;

public sealed record ResultDefinitionDto(
    Guid Id,
    string Unit,
    string ResultInstance,
    double? MinValue,
    double? MaxValue)
{
    public static ResultDefinitionDto FromDomain(
        ResultDefinition resultDefinition)
    {
        return new ResultDefinitionDto(resultDefinition.Id.Value, resultDefinition.Unit,
            resultDefinition.ResultInstance, resultDefinition.Specification.MinValue,
            resultDefinition.Specification.MaxValue);
    }
}
