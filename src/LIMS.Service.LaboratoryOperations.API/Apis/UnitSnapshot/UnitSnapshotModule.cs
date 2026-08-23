using Carter;
using LIMS.Service.LaboratoryOperations.Application.Units;
using Microsoft.AspNetCore.Mvc;

namespace LIMS.Service.LaboratoryOperations.API.Apis.UnitSnapshot;

public class UnitSnapshotModule
    : ModuleBase,
        ICarterModule
{
    public void AddRoutes(
        IEndpointRouteBuilder app)
    {
        var group = app.MapGroup("/api/unit-snapshots")
            .WithTags("UnitSnapshots");

        group.MapGet("/", GetAll)
            .Produces<ICollection<UnitSnapshotDto>>();

        group.MapGet("/{unitId:guid}", GetById)
            .Produces<UnitSnapshotDto>()
            .Produces(StatusCodes.Status404NotFound);
    }

    private static async Task<IResult> GetAll(
        [FromServices] UnitSnapshotServices services,
        CancellationToken cancellationToken = default)
    {
        var units = await services.Queries.GetAllAsync(cancellationToken);
        return Results.Ok(units);
    }

    private static async Task<IResult> GetById(
        Guid unitId,
        [FromServices] UnitSnapshotServices services,
        CancellationToken cancellationToken = default)
    {
        var unit = await services.Queries.GetByIdAsync(unitId, cancellationToken);
        return unit is not null ? Results.Ok(unit) : Results.NotFound();
    }
}
