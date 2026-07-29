namespace LIMS.DDD.Service.Application.StudyTemplates.CalculationRules.Commands;

public sealed record UpdateCalculationRuleCommand(
    string? Name,
    string? FormulaExpression,
    string? Description,
    Guid? ResultDefinitionId);
