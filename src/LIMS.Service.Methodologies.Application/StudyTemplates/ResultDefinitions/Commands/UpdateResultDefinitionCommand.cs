namespace LIMS.Service.Methodologies.Application.StudyTemplates.ResultDefinitions.Commands;

public sealed record UpdateResultDefinitionCommand(
    string? ResultInstance,
    Guid? UnitId,
    double? MinValue,
    double? MaxValue);
