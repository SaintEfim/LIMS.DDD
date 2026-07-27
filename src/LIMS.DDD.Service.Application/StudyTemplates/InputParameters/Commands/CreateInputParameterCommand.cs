namespace LIMS.DDD.Service.Application.StudyTemplates.InputParameters.Commands;

public sealed record CreateInputParameterCommand(
    string Name,
    string Description,
    string AliasName,
    double? MinValue,
    double? MaxValue);
