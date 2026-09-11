using Guides.Service.Application.Commands;

namespace Guides.Service.Apis;

public class UnitServices(UnitCommandsHandler commands, UnitQueries queries)
{
    public UnitCommandsHandler Commands { get; } = commands;
    public UnitQueries Queries { get; } = queries;
}
