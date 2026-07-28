using LIMS.DDD.Service.Domain.StudyTemplateAggregate.InputParameters;

namespace LIMS.DDD.Service.Domain.StudyTemplateAggregate.CalculationRules;

public sealed record CalculationInput
{
    private CalculationInput()
    {
    }

    internal CalculationInput(
        AliasName variableAlias,
        InputParameterId parameterId)
    {
        VariableAlias = variableAlias;
        ParameterId = parameterId;
    }

    public AliasName VariableAlias { get; private set; }

    public InputParameterId ParameterId { get; private set; }
}
