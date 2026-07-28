using LIMS.DDD.Service.Domain.StudyTemplateAggregate.Entities.InputParameters;
using LIMS.DDD.Service.Domain.StudyTemplateAggregate.Ids;

namespace LIMS.DDD.Service.Domain.StudyTemplateAggregate.Entities.CalculationRules;

public sealed record CalculationInput(AliasName VariableAlias, InputParameterId ParameterId);
