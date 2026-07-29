namespace LIMS.DDD.Service.Application.StudyTemplates.InputParameters.Commands;

public sealed record UpdateInputParameterCommand(
    string? Name,
    string? Description,
    string? AliasName,
    double? MinValue,
    double? MaxValue);
