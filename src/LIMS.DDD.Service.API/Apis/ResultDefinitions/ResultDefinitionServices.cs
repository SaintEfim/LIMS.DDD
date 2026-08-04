using LIMS.DDD.Service.Application.StudyTemplates.ResultDefinitions;
using LIMS.DDD.Service.Application.StudyTemplates.ResultDefinitions.Commands;

namespace LIMS.DDD.Service.API.Apis.ResultDefinitions;

public class ResultDefinitionServices(ResultDefinitionCommands commands, ResultDefinitionQueries queries)
{
    public ResultDefinitionCommands Commands { get; } = commands;

    public ResultDefinitionQueries Queries { get; } = queries;
}
