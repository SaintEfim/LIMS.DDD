using Service.Guides.Application.Commands;

namespace Service.Guides.API.Apis;

public class UnitServices(UnitCommandsHandler commands, UnitQueries queries)
{
    public UnitCommandsHandler Commands { get; } = commands;
    public UnitQueries Queries { get; } = queries;
}
