using LIMS.Service.LaboratoryOperations.Application.Studies.Core;

namespace LIMS.Service.LaboratoryOperations.API.Apis.Studies;

public class StudyServices(StudyCommandsHandler commands, StudyQueries queries)
{
    public StudyCommandsHandler Commands { get; } = commands;
    public StudyQueries Queries { get; } = queries;
}
