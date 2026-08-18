namespace LIMS.Service.LaboratoryOperations.Application.StudyTemplates;

public record CalculationRuleDto(
    Guid Id,
    string Name,
    string Description,
    string FormulaExpression,
    Guid ResultDefinitionId);
