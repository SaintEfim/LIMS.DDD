using LIMS.Service.LaboratoryOperations.Application.Units;

namespace LIMS.Service.LaboratoryOperations.API.Apis.UnitSnapshot;

public class UnitSnapshotServices(UnitSnapshotCommandsHandler commands, UnitSnapshotQueries queries)
{
    public UnitSnapshotCommandsHandler Commands { get; } = commands;
    public UnitSnapshotQueries Queries { get; } = queries;
}
