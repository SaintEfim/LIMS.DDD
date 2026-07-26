using LIMS.DDD.Service.Application.StudyTemplates.StudyTemplateDeterminations.Commands;
using LIMS.DDD.Service.Application.StudyTemplates.StudyTemplateDeterminations.Queries;

namespace LIMS.DDD.Service.API.Apis.StudyTemplateDeterminations;

public class StudyTemplateDeterminationServices(StudyTemplateDeterminationCommands commands, StudyTemplateDeterminationQueries queries)
{
    public StudyTemplateDeterminationCommands Commands { get; } = commands;

    public StudyTemplateDeterminationQueries Queries { get; } = queries;
}
