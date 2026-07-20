namespace LIMS.DDD.Service.Application.StudyTemplates.Commands;

public sealed record UpdateStudyTemplateCommand(string? Name, string? Description, string? Revision);
