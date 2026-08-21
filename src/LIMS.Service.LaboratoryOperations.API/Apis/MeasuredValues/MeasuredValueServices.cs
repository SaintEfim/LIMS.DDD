using LIMS.Service.LaboratoryOperations.Application.Studies.MeasuredValues;

namespace LIMS.Service.LaboratoryOperations.API.Apis.MeasuredValues;

public class MeasuredValueServices(MeasuredValueCommandsHandler commands, MeasuredValueQueries queries)
{
    public MeasuredValueCommandsHandler Commands { get; } = commands;
    public MeasuredValueQueries Queries { get; } = queries;
}
