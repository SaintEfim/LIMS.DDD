using LIMS.Service.LaboratoryOperations.Application.Samples;

namespace LIMS.Service.LaboratoryOperations.API.Apis.Samples;

public class SampleServices(SampleCommandsHandler commands, SampleQueries queries)
{
    public SampleCommandsHandler Commands { get; } = commands;
    public SampleQueries Queries { get; } = queries;
}
