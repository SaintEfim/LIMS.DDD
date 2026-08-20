using Domain.SeedWork.SeedWork.Result;
using Domain.SeedWork.SeedWork.SoftDeletable;
using Domain.SeedWork.SeedWork.ValueObjects;
using LIMS.Service.Methodologies.Domain.StudyTemplateAggregate.Entities.InputParameters;
using LIMS.Service.Methodologies.Domain.StudyTemplateAggregate.Entities.ResultDefinitions;

namespace LIMS.Service.Methodologies.Domain.StudyTemplateAggregate.Entities.CalculationRules;

public sealed class CalculationRule : SoftDeletableModel
{
    internal CalculationRule(
        StudyTemplateId studyTemplateId,
        Name name,
        FormulaExpression formulaExpression,
        Description description,
        ResultDefinitionId resultDefinitionId)
    {
        Id = new CalculationRuleId(Guid.NewGuid());
        StudyTemplateId = studyTemplateId;
        Name = name;
        FormulaExpression = formulaExpression;
        Description = description;
        ResultDefinitionId = resultDefinitionId;
    }

    public ResultDefinitionId ResultDefinitionId { get; private set; }

    public CalculationRuleId Id { get; private set; }

    public StudyTemplateId StudyTemplateId { get; private set; }

    public Name Name { get; private set; }

    public FormulaExpression FormulaExpression { get; private set; }

    public Description Description { get; private set; }

    internal void Update(
        Name? name,
        FormulaExpression? formulaExpression,
        Description? description,
        ResultDefinitionId? resultDefinitionId)
    {
        if (name is not null)
        {
            Name = name;
        }

        if (formulaExpression is not null)
        {
            FormulaExpression = formulaExpression;
        }

        if (description is not null)
        {
            Description = description;
        }

        if (resultDefinitionId is not null)
        {
            ResultDefinitionId = resultDefinitionId.Value;
        }
    }

    internal void MarkAsDeleted()
    {
        IsDeleted = true;
        DeletedAt = DateTimeOffset.UtcNow;
    }

    internal Result<None, Exception> ValidateVariables(
        IReadOnlyCollection<InputParameter> templateParameters)
    {
        var variables = FormulaExpression.ExtractVariables();

        foreach (var variable in variables)
        {
            var parameter = templateParameters.FirstOrDefault(p => p.AliasName.Value == variable && !p.IsDeleted);

            if (parameter is null)
            {
                return new InvalidOperationException(
                    $"Calculation rule '{Name.Value}': variable '{variable}' references.");
            }
        }

        return new None();
    }
}
