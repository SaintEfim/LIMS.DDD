using LIMS.DDD.Service.Application.StudyTemplates.StudyTemplateObservations.Commands;
using LIMS.DDD.Service.Application.StudyTemplates.StudyTemplateObservations.Queries;

namespace LIMS.DDD.Service.API.Apis.StudyTemplateObservations;

public class StudyTemplateObservationServices(
    StudyTemplateObservationCommands commands,
    StudyTemplateObservationQueries queries)
{
    public StudyTemplateObservationCommands Commands { get; } = commands;

    public StudyTemplateObservationQueries Queries { get; } = queries;
}
