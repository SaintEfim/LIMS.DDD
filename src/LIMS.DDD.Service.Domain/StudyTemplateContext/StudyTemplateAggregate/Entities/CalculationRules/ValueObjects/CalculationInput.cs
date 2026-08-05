using LIMS.DDD.Service.Domain.SeedWork.ValueObjects;
using LIMS.DDD.Service.Domain.StudyTemplateContext.StudyTemplateAggregate.Entities.InputParameters;
using LIMS.DDD.Service.Domain.StudyTemplateContext.StudyTemplateAggregate.ValueObjects;

namespace LIMS.DDD.Service.Domain.StudyTemplateContext.StudyTemplateAggregate.Entities.CalculationRules.ValueObjects;

public sealed record CalculationInput(AliasName VariableAlias, InputParameterId ParameterId);
