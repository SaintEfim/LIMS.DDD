using LIMS.Service.Methodologies.Application.StudyTemplates.Core;

namespace LIMS.Service.Methodologies.API.Apis.StudyTemplates;

public class StudyTemplateServices(StudyTemplateCommandsHandler commands, StudyTemplateQueries queries)
{
    public StudyTemplateCommandsHandler Commands { get; } = commands;

    public StudyTemplateQueries Queries { get; } = queries;
}
