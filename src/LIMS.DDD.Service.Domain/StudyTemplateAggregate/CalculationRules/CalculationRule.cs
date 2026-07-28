using LIMS.DDD.Service.Domain.SeedWork;
using LIMS.DDD.Service.Domain.SeedWork.Result;
using LIMS.DDD.Service.Domain.StudyTemplateAggregate.InputParameters;
using LIMS.DDD.Service.Domain.StudyTemplateAggregate.ResultDefinitions;

namespace LIMS.DDD.Service.Domain.StudyTemplateAggregate.CalculationRules;

public readonly record struct CalculationRuleId(Guid Value) : IValueObjectId
{
    public Guid Value { get; } = Value;
}

public sealed class CalculationRule
{
    private CalculationRule()
    {
    }

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

    public CalculationRuleId Id { get; private set; }

    public StudyTemplateId StudyTemplateId { get; private set; }

    public ResultDefinitionId ResultDefinitionId;

    public IReadOnlyCollection<CalculationInput> CalculationInputs => _calculationInputs.AsReadOnly();

    private readonly HashSet<CalculationInput> _calculationInputs = [];

    public Name Name { get; private set; }

    public FormulaExpression FormulaExpression { get; private set; }

    public Description Description { get; private set; }

    internal Result<Exception> AddInput(
        AliasName variableAlias,
        InputParameterId inputParameterId)
    {
        if (_calculationInputs.Any(i => i.VariableAlias == variableAlias))
        {
            return Result<Exception>.Failure(
                new InvalidOperationException("Variable alias must be unique within the calculation rule."));
        }

        _calculationInputs.Add(new CalculationInput(variableAlias, inputParameterId));

        return Result<Exception>.Success();
    }

    internal Result<Exception> RemoveInput(
        AliasName variableAlias)
    {
        var inputToRemove = _calculationInputs.FirstOrDefault(i => i.VariableAlias == variableAlias);

        if (inputToRemove == null)
        {
            return Result<Exception>.Failure(
                new InvalidOperationException("Variable alias must be unique within the calculation rule."));
        }

        _calculationInputs.Remove(inputToRemove);
        return Result<Exception>.Success();
    }
}
