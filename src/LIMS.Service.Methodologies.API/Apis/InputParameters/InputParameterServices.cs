using LIMS.Service.Methodologies.Application.StudyTemplates.InputParameters;

namespace LIMS.Service.Methodologies.API.Apis.InputParameters;

public class InputParameterServices(InputParameterCommandsHandler commands, InputParameterQueries queries)
{
    public InputParameterCommandsHandler Commands { get; } = commands;

    public InputParameterQueries Queries { get; } = queries;
}
