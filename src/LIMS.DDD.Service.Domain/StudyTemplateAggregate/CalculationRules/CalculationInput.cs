using LIMS.DDD.Service.Domain.SeedWork;
using LIMS.DDD.Service.Domain.StudyTemplateAggregate.InputParameters;

namespace LIMS.DDD.Service.Domain.StudyTemplateAggregate.CalculationRules;

public readonly record struct CalculationInputId(Guid Value) : IValueObjectId
{
    public Guid Value { get; } = Value;
}

public sealed record CalculationInput
{
    private CalculationInput()
    {
    }

    internal CalculationInput(
        AliasName variableAlias,
        InputParameterId parameterId)
    {
        Id = new CalculationInputId(Guid.NewGuid());
        VariableAlias = variableAlias;
        ParameterId = parameterId;
    }

    public CalculationInputId Id { get; private set; }

    public AliasName VariableAlias { get; private set; }

    public InputParameterId ParameterId { get; private set; }
}
