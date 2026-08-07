using LIMS.DDD.Service.Application.StudyTemplates.Core;

namespace LIMS.DDD.Service.API.Apis.StudyTemplates;

public class StudyTemplateServices(StudyTemplateCommandsHandler commands, StudyTemplateQueries queries)
{
    public StudyTemplateCommandsHandler Commands { get; } = commands;

    public StudyTemplateQueries Queries { get; } = queries;
}
