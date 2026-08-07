using LIMS.DDD.Service.Application.Studies.MeasuredValues;

namespace LIMS.DDD.Service.API.Apis.MeasuredValues;

public class MeasuredValueServices(MeasuredValueCommandsHandler commands, MeasuredValueQueries queries)
{
    public MeasuredValueCommandsHandler Commands { get; } = commands;
    public MeasuredValueQueries Queries { get; } = queries;
}
