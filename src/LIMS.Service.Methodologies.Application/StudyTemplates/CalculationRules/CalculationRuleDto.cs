using LIMS.Service.Methodologies.Domain.StudyTemplateAggregate.Entities.CalculationRules;

namespace LIMS.Service.Methodologies.Application.StudyTemplates.CalculationRules;

public sealed record CalculationRuleDto(
    Guid Id,
    string Name,
    string FormulaExpression,
    string? Description,
    Guid ResultDefinitionId)
{
    public static CalculationRuleDto FromDomain(
        CalculationRule rule)
    {
        return new CalculationRuleDto(rule.Id.Value, rule.Name.Value, rule.FormulaExpression.Value,
            rule.Description.Value, rule.ResultDefinitionId.Value);
    }
}
