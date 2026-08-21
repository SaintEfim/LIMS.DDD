using Domain.SeedWork.SeedWork;
using Domain.SeedWork.SeedWork.ValueObjects;
using LIMS.Service.LaboratoryOperations.Domain.StudyTemplateSnapshots.ResultDefinitions;

namespace LIMS.Service.LaboratoryOperations.Domain.StudyTemplateSnapshots.CalculationRules;

public sealed class CalculationRuleSnapshot : SoftDeletableModel
{
    private CalculationRuleSnapshot()
    {
    }

    public CalculationRuleSnapshot(
        CalculationRuleId id,
        StudyTemplateId studyTemplateId,
        Name name,
        Description description,
        FormulaExpression formulaExpression,
        ResultDefinitionId resultDefinitionId)
    {
        Id = id;
        StudyTemplateId = studyTemplateId;
        Name = name;
        Description = description;
        FormulaExpression = formulaExpression;
        ResultDefinitionId = resultDefinitionId;
    }

    public CalculationRuleId Id { get; private set; }
    public StudyTemplateId StudyTemplateId { get; private set; }
    public Name Name { get; private set; } = null!;
    public Description Description { get; private set; } = null!;
    public FormulaExpression FormulaExpression { get; private set; } = null!;
    public ResultDefinitionId ResultDefinitionId { get; private set; }
}
