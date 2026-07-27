using LIMS.DDD.Service.Domain.StudyTemplateAggregate.InputParameters;

namespace LIMS.DDD.Service.Domain.StudyTemplateAggregate.CalculationRules;

public sealed record CalculationInput(AliasName VariableAlias, InputParameterId ParameterId);
