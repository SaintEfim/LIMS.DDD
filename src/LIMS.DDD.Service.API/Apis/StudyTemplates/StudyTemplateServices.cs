using LIMS.DDD.Service.Application.StudyTemplates.Commands;
using LIMS.DDD.Service.Application.StudyTemplates.Queries;

namespace LIMS.DDD.Service.API.Apis.StudyTemplates;

public class StudyTemplateServices(StudyTemplateCommands commands, StudyTemplateQueries queries)
{
    public StudyTemplateCommands Commands { get; } = commands;

    public StudyTemplateQueries Queries { get; } =  queries;
}

