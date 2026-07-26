namespace LIMS.DDD.Service.Application.StudyTemplates.StudyTemplateObservations.Commands;

public sealed record CreateStudyTemplateObservationCommand(
    string Name,
    string Description,
    string AliasName,
    double? MinValue,
    double? MaxValue);
