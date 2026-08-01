using LIMS.DDD.Service.Domain.StudyTemplateContext.StudyTemplateAggregate.Ids;

namespace LIMS.DDD.Service.Domain.StudyTemplateContext.StudyTemplateAggregate.ValueObjects;

public sealed record CalculationInput(AliasName VariableAlias, InputParameterId ParameterId);
