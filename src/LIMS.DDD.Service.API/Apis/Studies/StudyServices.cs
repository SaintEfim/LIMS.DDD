using LIMS.DDD.Service.Application.Studies.Core;

namespace LIMS.DDD.Service.API.Apis.Studies;

public class StudyServices(StudyCommandsHandler commands, StudyQueries queries)
{
    public StudyCommandsHandler Commands { get; } = commands;
    public StudyQueries Queries { get; } = queries;
}
