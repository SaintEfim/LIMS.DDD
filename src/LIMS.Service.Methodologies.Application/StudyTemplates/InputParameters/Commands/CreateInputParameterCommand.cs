namespace LIMS.Service.Methodologies.Application.StudyTemplates.InputParameters.Commands;

public sealed record CreateInputParameterCommand(
    string Name,
    string Description,
    string AliasName,
    double? MinValue,
    double? MaxValue);
