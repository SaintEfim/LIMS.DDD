using LIMS.Service.Methodologies.Application.StudyTemplates.ResultDefinitions;

namespace LIMS.Service.Methodologies.API.Apis.ResultDefinitions;

public class ResultDefinitionServices(ResultDefinitionCommandsHandler commands, ResultDefinitionQueries queries)
{
    public ResultDefinitionCommandsHandler Commands { get; } = commands;

    public ResultDefinitionQueries Queries { get; } = queries;
}
