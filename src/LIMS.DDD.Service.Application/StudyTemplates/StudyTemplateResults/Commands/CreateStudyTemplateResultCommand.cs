namespace LIMS.DDD.Service.Application.StudyTemplates.StudyTemplateResults.Commands;

public sealed record CreateStudyTemplateResultCommand(
    string Unit,
    double? MinValue,
    double? MaxValue);
