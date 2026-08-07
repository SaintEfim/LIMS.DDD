using LIMS.DDD.Service.Application.StudyTemplates.ResultDefinitions;

namespace LIMS.DDD.Service.API.Apis.ResultDefinitions;

public class ResultDefinitionServices(ResultDefinitionCommandsHandler commands, ResultDefinitionQueries queries)
{
    public ResultDefinitionCommandsHandler Commands { get; } = commands;

    public ResultDefinitionQueries Queries { get; } = queries;
}
