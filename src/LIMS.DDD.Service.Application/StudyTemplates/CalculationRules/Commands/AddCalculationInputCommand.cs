namespace LIMS.DDD.Service.Application.StudyTemplates.CalculationRules.Commands;

public sealed record AddCalculationInputCommand(
    string VariableAlias,
    Guid InputParameterId);
