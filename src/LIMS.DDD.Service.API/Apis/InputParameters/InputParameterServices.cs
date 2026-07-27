using LIMS.DDD.Service.Application.StudyTemplates.InputParameters.Commands;
using LIMS.DDD.Service.Application.StudyTemplates.InputParameters.Queries;

namespace LIMS.DDD.Service.API.Apis.InputParameters;

public class InputParameterServices(
    InputParameterCommands commands,
    InputParameterQueries queries)
{
    public InputParameterCommands Commands { get; } = commands;

    public InputParameterQueries Queries { get; } = queries;
}
