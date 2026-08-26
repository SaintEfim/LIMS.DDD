using LIMS.Service.LaboratoryOperations.Domain.StudyTemplateSnapshots.CalculationRules;

namespace LIMS.Service.LaboratoryOperations.Application.StudyTemplates;

public sealed record CalculationRuleDto(
    Guid Id,
    string Name,
    string? Description,
    string FormulaExpression,
    Guid ResultDefinitionId)
{
    public static CalculationRuleDto FromSnapshot(
        CalculationRuleSnapshot snapshot)
    {
        return new CalculationRuleDto(snapshot.Id.Value, snapshot.Name.Value, snapshot.Description.Value,
            snapshot.FormulaExpression.Value, snapshot.ResultDefinitionId.Value);
    }
}
