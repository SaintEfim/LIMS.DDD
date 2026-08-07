using LIMS.DDD.Service.Application.StudyTemplates.InputParameters;

namespace LIMS.DDD.Service.API.Apis.InputParameters;

public class InputParameterServices(InputParameterCommandsHandler commands, InputParameterQueries queries)
{
    public InputParameterCommandsHandler Commands { get; } = commands;

    public InputParameterQueries Queries { get; } = queries;
}
