using Carter;
using LIMS.Service.Methodologies.Application.Units;
using Microsoft.AspNetCore.Mvc;

namespace LIMS.Service.Methodologies.API.Apis.UnitSnapshot;

public class UnitSnapshotModule : ICarterModule
{
    public void AddRoutes(
        IEndpointRouteBuilder app)
    {
        var group = app.MapGroup("/api/unit-snapshots")
            .WithTags("UnitSnapshots");

        group.MapGet("/", GetAll)
            .Produces<ICollection<UnitDto>>();

        group.MapGet("/{unitId:guid}", GetById)
            .Produces<UnitDto>()
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
