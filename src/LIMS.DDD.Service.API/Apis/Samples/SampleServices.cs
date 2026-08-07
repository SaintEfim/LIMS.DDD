using LIMS.DDD.Service.Application.Samples;

namespace LIMS.DDD.Service.API.Apis.Samples;

public class SampleServices(SampleCommandsHandler commands, SampleQueries queries)
{
    public SampleCommandsHandler Commands { get; } = commands;
    public SampleQueries Queries { get; } = queries;
}
