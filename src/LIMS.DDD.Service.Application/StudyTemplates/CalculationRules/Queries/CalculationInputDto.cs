using LIMS.DDD.Service.Domain.StudyTemplateAggregate.CalculationRules;

namespace LIMS.DDD.Service.Application.StudyTemplates.CalculationRules.Queries;

public sealed record CalculationInputDto(string VariableAlias, Guid ParameterId)
{
    public static CalculationInputDto FromDomain(
        CalculationInput input)
    {
        return new CalculationInputDto(VariableAlias: input.VariableAlias.Value, ParameterId: input.ParameterId.Value);
    }
}
