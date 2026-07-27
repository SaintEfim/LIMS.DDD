namespace LIMS.DDD.Service.Application.StudyTemplates.ResultDefinitions.Commands;

public sealed record CreateResultDefinitionCommand(
    string ResultInstance,
    string Unit,
    double? MinValue,
    double? MaxValue);
