using LIMS.DDD.Service.Domain.StudyTemplateContext.StudyTemplateAggregate.Entities;

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
        return new CalculationRuleDto(Id: rule.Id.Value, Name: rule.Name.Value,
            FormulaExpression: rule.FormulaExpression.Value, Description: rule.Description.Value,
            ResultDefinitionId: rule.ResultDefinitionId.Value, Inputs: rule.CalculationInputs
                .Select(CalculationInputDto.FromDomain)
                .ToList());
    }
}
