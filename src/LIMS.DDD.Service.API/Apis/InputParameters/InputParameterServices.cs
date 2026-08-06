using LIMS.DDD.Service.Application.StudyTemplates.InputParameters;

namespace LIMS.DDD.Service.API.Apis.InputParameters;

public class InputParameterServices(InputParameterCommandHandler commands, InputParameterQueries queries)
{
    public InputParameterCommandHandler Commands { get; } = commands;

    public InputParameterQueries Queries { get; } = queries;
}
