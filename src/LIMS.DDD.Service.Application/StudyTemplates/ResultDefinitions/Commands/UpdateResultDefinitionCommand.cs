namespace LIMS.DDD.Service.Application.StudyTemplates.ResultDefinitions.Commands;

public sealed record UpdateResultDefinitionCommand(
    string? ResultInstance,
    string? Unit,
    double? MinValue,
    double? MaxValue);
