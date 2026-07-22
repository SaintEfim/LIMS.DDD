namespace LIMS.DDD.Service.Application.StudyTemplates.StudyTemplateResults.Commands;

public sealed record CreateStudyTemplateResultCommand(
    string ResultInstance,
    string Unit,
    double? MinValue,
    double? MaxValue);
