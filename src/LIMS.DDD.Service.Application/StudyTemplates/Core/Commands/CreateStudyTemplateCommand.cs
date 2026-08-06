namespace LIMS.DDD.Service.Application.StudyTemplates.Core.Commands;

public sealed record CreateStudyTemplateCommand(string Name, string Description, string Revision);
