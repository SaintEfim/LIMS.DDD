using Domain.SeedWork.SeedWork.SoftDeletable;
using Domain.SeedWork.SeedWork.ValueObjects;
using LIMS.Service.LaboratoryOperations.Domain.StudyTemplateSnapshots.ResultDefinitions;

namespace LIMS.Service.LaboratoryOperations.Domain.StudyTemplateSnapshots.CalculationRules;

public sealed record CalculationRuleSnapshot(
    CalculationRuleId Id,
    StudyTemplateId StudyTemplateId,
    Name Name,
    Description Description,
    FormulaExpression FormulaExpression,
    ResultDefinitionId ResultDefinitionId) : SoftDeletableRecord;
