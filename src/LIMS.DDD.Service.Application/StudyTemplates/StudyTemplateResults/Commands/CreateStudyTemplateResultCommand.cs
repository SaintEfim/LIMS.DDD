namespace LIMS.DDD.Service.Application.StudyTemplates.StudyTemplateResults.Commands;

public sealed record CreateStudyTemplateResultCommand(
    Guid StudyTemplateId,
    string Unit,
    double? MinValue,
    double? MaxValue);
