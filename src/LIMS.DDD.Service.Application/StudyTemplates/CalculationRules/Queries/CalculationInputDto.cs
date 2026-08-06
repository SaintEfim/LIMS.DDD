using LIMS.DDD.Service.Domain.StudyTemplateContext.StudyTemplateAggregate.Entities.CalculationRules.ValueObjects;

namespace LIMS.DDD.Service.Application.StudyTemplates.CalculationRules.Queries;

public sealed record CalculationInputDto(string VariableAlias, Guid ParameterId)
{
    public static CalculationInputDto FromDomain(
        CalculationInput input)
    {
        return new CalculationInputDto(input.VariableAlias.Value, input.ParameterId.Value);
    }
}
