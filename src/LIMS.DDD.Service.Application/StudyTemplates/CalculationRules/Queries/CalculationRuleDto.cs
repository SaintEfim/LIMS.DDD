using LIMS.DDD.Service.Domain.StudyTemplateContext.StudyTemplateAggregate.Entities.CalculationRules;

namespace LIMS.DDD.Service.Application.StudyTemplates.CalculationRules.Queries;

public sealed record CalculationRuleDto(
    Guid Id,
    string Name,
    string FormulaExpression,
    string? Description,
    Guid ResultDefinitionId,
    ICollection<CalculationInputDto> Inputs)
{
    public static CalculationRuleDto FromDomain(
        CalculationRule rule)
    {
        return new CalculationRuleDto(rule.Id.Value, rule.Name.Value, rule.FormulaExpression.Value,
            rule.Description.Value, rule.ResultDefinitionId.Value, rule.CalculationInputs
                .Select(CalculationInputDto.FromDomain)
                .ToList());
    }
}
