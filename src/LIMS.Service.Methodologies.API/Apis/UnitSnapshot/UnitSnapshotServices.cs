using LIMS.Service.Methodologies.Application.Units;

namespace LIMS.Service.Methodologies.API.Apis.UnitSnapshot;

public class UnitSnapshotServices(
    UnitSnapshotCommandsHandler commands,
    UnitSnapshotQueries queries)
{
    public UnitSnapshotCommandsHandler Commands { get; } = commands;
    public UnitSnapshotQueries Queries { get; } = queries;
}
