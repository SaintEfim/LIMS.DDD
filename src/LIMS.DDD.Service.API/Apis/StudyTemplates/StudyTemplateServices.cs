using LIMS.DDD.Service.Application.StudyTemplates.Core;

namespace LIMS.DDD.Service.API.Apis.StudyTemplates;

public class StudyTemplateServices(StudyTemplateCommandHandler commands, StudyTemplateQueries queries)
{
    public StudyTemplateCommandHandler Commands { get; } = commands;

    public StudyTemplateQueries Queries { get; } = queries;
}
