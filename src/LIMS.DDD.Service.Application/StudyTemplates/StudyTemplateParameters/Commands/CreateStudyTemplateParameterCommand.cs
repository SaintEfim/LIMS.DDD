namespace LIMS.DDD.Service.Application.StudyTemplates.StudyTemplateParameters.Commands;

public sealed record CreateStudyTemplateParameterCommand(
    Guid StudyTemplateId,
    string Name,
    string Description,
    string AliasName,
    double? MinValue,
    double? MaxValue);
