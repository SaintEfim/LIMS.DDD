namespace LIMS.DDD.Service.Application.StudyTemplates.StudyTemplateDeterminations.Commands;

public sealed record CreateStudyTemplateDeterminationCommand(
    string ResultInstance,
    string Unit,
    double? MinValue,
    double? MaxValue);
