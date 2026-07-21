using LIMS.DDD.Service.Application.StudyTemplates.StudyTemplateParameters.Commands;
using LIMS.DDD.Service.Application.StudyTemplates.StudyTemplateParameters.Queries;

namespace LIMS.DDD.Service.API.Apis.StudyTemplateParameters;

public class StudyTemplateParameterServices(
    StudyTemplateParameterCommands commands,
    StudyTemplateParameterQueries queries)
{
    public StudyTemplateParameterCommands Commands { get; } = commands;

    public StudyTemplateParameterQueries Queries { get; } = queries;
}
