using LIMS.DDD.Service.Domain.StudyTemplateAggregate.CalculationRules;

namespace LIMS.DDD.Service.Application.StudyTemplates.CalculationRules.Queries;

public sealed record CalculationRuleDto(
    Guid Id,
    string Name,
    string FormulaExpression,
    string? Description,
    ICollection<CalculationInputDto> Inputs)
{
    public static CalculationRuleDto FromDomain(CalculationRule rule)
    {
        return new CalculationRuleDto(
            Id: rule.Id.Value,
            Name: rule.Name.Value,
            FormulaExpression: rule.FormulaExpression.Value,
            Description: rule.Description.Value,
            Inputs: rule.CalculationInputs.Select(CalculationInputDto.FromDomain).ToList()
        );
    }
}
