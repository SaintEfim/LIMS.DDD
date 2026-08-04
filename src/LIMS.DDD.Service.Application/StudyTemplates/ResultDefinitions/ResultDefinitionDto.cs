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
        return new ResultDefinitionDto(Id: resultDefinition.Id.Value, Unit: resultDefinition.Unit,
            ResultInstance: resultDefinition.ResultInstance, MinValue: resultDefinition.Specification.MinValue,
            MaxValue: resultDefinition.Specification.MaxValue);
    }
}
