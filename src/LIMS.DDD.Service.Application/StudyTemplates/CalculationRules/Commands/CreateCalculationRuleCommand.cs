namespace LIMS.DDD.Service.Application.StudyTemplates.CalculationRules.Commands;

public sealed record CreateCalculationRuleCommand(
    string Name,
    string FormulaExpression,
    string Description);
