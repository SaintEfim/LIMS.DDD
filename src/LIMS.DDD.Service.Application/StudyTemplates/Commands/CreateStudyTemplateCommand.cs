namespace LIMS.DDD.Service.Application.StudyTemplates.Commands;

public sealed record CreateStudyTemplateCommand(
    string Name,
    string Description,
    string Revision);
