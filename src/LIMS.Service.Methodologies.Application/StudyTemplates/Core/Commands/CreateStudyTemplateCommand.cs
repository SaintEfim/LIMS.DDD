namespace LIMS.Service.Methodologies.Application.StudyTemplates.Core.Commands;

public sealed record CreateStudyTemplateCommand(string Name, string Description, string Revision);
