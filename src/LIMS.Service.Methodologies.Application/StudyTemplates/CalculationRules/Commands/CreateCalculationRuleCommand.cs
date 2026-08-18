namespace LIMS.Service.Methodologies.Application.StudyTemplates.CalculationRules.Commands;

public sealed record CreateCalculationRuleCommand(
    string Name,
    string FormulaExpression,
    string Description,
    Guid ResultDefinitionId);
