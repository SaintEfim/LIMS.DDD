using Carter;
using Guides.Service.Application.Commands;
using Microsoft.AspNetCore.Mvc;

namespace Guides.Service.API.Apis;

public class UnitModule
    : ModuleBase,
        ICarterModule
{
    public void AddRoutes(
        IEndpointRouteBuilder app)
    {
        var group = app.MapGroup("/api/units")
            .WithTags("Units");

        group.MapGet("/", GetAll)
            .Produces<ICollection<UnitDto>>();

        group.MapGet("/{id:guid}", GetById)
            .Produces<UnitDto>()
            .Produces(StatusCodes.Status404NotFound);
    }

    private static async Task<IResult> GetAll(
        [FromServices] UnitServices services,
        CancellationToken cancellationToken = default)
    {
        var units = await services.Queries.GetAllAsync(cancellationToken);
        return Results.Ok(units);
    }

    private static async Task<IResult> GetById(
        Guid id,
        [FromServices] UnitServices services,
        CancellationToken cancellationToken = default)
    {
        var dto = await services.Queries.GetByIdAsync(id, cancellationToken);
        return dto is null ? Results.NotFound() : Results.Ok(dto);
    }
}
