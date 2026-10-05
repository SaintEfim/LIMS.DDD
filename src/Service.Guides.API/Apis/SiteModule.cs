using Carter;
using Microsoft.AspNetCore.Mvc;
using Service.Guides.Application.Sites;
namespace Service.Guides.API.Apis;

public sealed class SiteModule : ModuleBase, ICarterModule
{
    public void AddRoutes(IEndpointRouteBuilder app)
    {
        var group = app.MapGroup("/api/sites").WithTags("Sites");
        group.MapGet("/", (SiteQueries queries, CancellationToken ct) => queries.GetAllAsync(ct));
        group.MapGet("/tree", (SiteQueries queries, CancellationToken ct) => queries.GetTreeAsync(ct));
        group.MapGet("/{id:guid}", async (Guid id, SiteQueries queries, CancellationToken ct) =>
        {
            var site = await queries.GetByIdAsync(id, ct);
            return site is null ? Results.NotFound() : Results.Ok(site);
        }).Produces<SiteDto>().Produces(StatusCodes.Status404NotFound);
        group.MapPost("/", async ([FromBody] CreateSiteCommand command, SiteCommandsHandler commands, CancellationToken ct) =>
        {
            var result = await commands.CreateAsync(command, ct);
            return result.IsFailure ? HandleFailure(result.GetError()) :
                Results.Created($"/api/sites/{result.GetValue().Id.Value}", SiteDto.FromDomain(result.GetValue()));
        }).Produces<SiteDto>(StatusCodes.Status201Created).Produces(StatusCodes.Status400BadRequest).Produces(StatusCodes.Status404NotFound);
        group.MapPut("/{id:guid}", async (Guid id, [FromBody] UpdateSiteCommand command, SiteCommandsHandler commands, CancellationToken ct) =>
        {
            var result = await commands.UpdateAsync(id, command, ct);
            return result.IsFailure ? HandleFailure(result.GetError()) : Results.Ok(SiteDto.FromDomain(result.GetValue()));
        }).Produces<SiteDto>().Produces(StatusCodes.Status400BadRequest).Produces(StatusCodes.Status404NotFound);
        group.MapDelete("/{id:guid}", async (Guid id, SiteCommandsHandler commands, CancellationToken ct) =>
        {
            var result = await commands.DeleteAsync(id, ct);
            return result.IsFailure ? HandleFailure(result.GetError()) : Results.NoContent();
        }).Produces(StatusCodes.Status204NoContent).Produces(StatusCodes.Status404NotFound).Produces(StatusCodes.Status409Conflict);
    }
}