using LIMS.DDD.Service.Application.StudyTemplates.StudyTemplateResults.Commands;
using LIMS.DDD.Service.Application.StudyTemplates.StudyTemplateResults.Queries;

namespace LIMS.DDD.Service.API.Apis.StudyTemplateResults;

public class StudyTemplateResultServices(StudyTemplateResultCommands commands, StudyTemplateResultQueries queries)
{
    public StudyTemplateResultCommands Commands { get; } = commands;

    public StudyTemplateResultQueries Queries { get; } = queries;
}
