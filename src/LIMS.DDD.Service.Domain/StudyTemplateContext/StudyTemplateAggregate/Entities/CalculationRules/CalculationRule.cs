using LIMS.DDD.Service.Domain.SeedWork;
using LIMS.DDD.Service.Domain.SeedWork.Result;
using LIMS.DDD.Service.Domain.SeedWork.ValueObjects;
using LIMS.DDD.Service.Domain.StudyTemplateContext.StudyTemplateAggregate.Entities.CalculationRules.ValueObjects;
using LIMS.DDD.Service.Domain.StudyTemplateContext.StudyTemplateAggregate.Entities.InputParameters;
using LIMS.DDD.Service.Domain.StudyTemplateContext.StudyTemplateAggregate.Entities.ResultDefinitions;

namespace LIMS.DDD.Service.Domain.StudyTemplateContext.StudyTemplateAggregate.Entities.CalculationRules;

public sealed class CalculationRule : SoftDeletableModel
{
    private readonly HashSet<CalculationInput> _calculationInputs = [];

    public ResultDefinitionId ResultDefinitionId;

    private CalculationRule()
    {
    }

    public CalculationRuleId Id { get; private set; }

    public StudyTemplateId StudyTemplateId { get; private set; }

    public IReadOnlyCollection<CalculationInput> CalculationInputs => _calculationInputs.AsReadOnly();

    public Name Name { get; private set; }

    public FormulaExpression FormulaExpression { get; private set; }

    public Description Description { get; private set; }

    internal static CalculationRule Create(
        StudyTemplateId studyTemplateId,
        Name name,
        FormulaExpression formulaExpression,
        Description description,
        ResultDefinitionId resultDefinitionId)
    {
        return new CalculationRule
        {
            Id = new CalculationRuleId(Guid.NewGuid()),
            StudyTemplateId = studyTemplateId,
            Name = name,
            FormulaExpression = formulaExpression,
            Description = description,
            ResultDefinitionId = resultDefinitionId
        };
    }

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

    internal Result<UnitEmpty, Exception> AddInput(
        AliasName variableAlias,
        InputParameterId inputParameterId)
    {
        if (_calculationInputs.Any(i => i.VariableAlias == variableAlias))
        {
            return Result<UnitEmpty, Exception>.Failure(
                new InvalidOperationException("Variable alias must be unique within the calculation rule."));
        }

        _calculationInputs.Add(new CalculationInput(variableAlias, inputParameterId));

        return Result<UnitEmpty, Exception>.Success(new UnitEmpty());
    }

    internal Result<UnitEmpty, Exception> RemoveInput(
        AliasName variableAlias)
    {
        var inputToRemove = _calculationInputs.FirstOrDefault(i => i.VariableAlias == variableAlias);

        if (inputToRemove == null)
        {
            return Result<UnitEmpty, Exception>.Failure(
                new InvalidOperationException("Variable alias must be unique within the calculation rule."));
        }

        _calculationInputs.Remove(inputToRemove);
        return Result<UnitEmpty, Exception>.Success(new UnitEmpty());
    }
}
