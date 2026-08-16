using Carter;
using LIMS.DDD.Service.Application.Studies.MeasuredValues;
using Microsoft.AspNetCore.Mvc;

namespace LIMS.DDD.Service.API.Apis.MeasuredValues;

public class MeasuredValueModule : ICarterModule
{
    public void AddRoutes(
        IEndpointRouteBuilder app)
    {
        var group = app.MapGroup("/api/studies/{studyId:guid}/measured-values")
            .WithTags("MeasuredValues");

        group.MapGet("/", GetAll)
            .Produces<ICollection<MeasuredValueDto>>();

        group.MapGet("/{measuredValueId:guid}", GetById)
            .Produces<MeasuredValueDto>()
            .Produces(StatusCodes.Status404NotFound);

        group.MapPatch("/{measuredValueId:guid}", Update)
            .Produces(StatusCodes.Status204NoContent)
            .Produces(StatusCodes.Status404NotFound)
            .Produces(StatusCodes.Status400BadRequest);
    }

    private static async Task<IResult> GetAll(
        Guid studyId,
        [FromServices] MeasuredValueServices services,
        CancellationToken cancellationToken = default)
    {
        var values = await services.Queries.GetAllByStudyIdAsync(studyId, cancellationToken);
        return Results.Ok(values);
    }

    private static async Task<IResult> GetById(
        Guid studyId,
        Guid measuredValueId,
        [FromServices] MeasuredValueServices services,
        CancellationToken cancellationToken = default)
    {
        var dto = await services.Queries.GetByIdAsync(studyId, measuredValueId, cancellationToken);
        return dto is not null ? Results.Ok(dto) : Results.NotFound();
    }

    private static async Task<IResult> Update(
        Guid studyId,
        Guid measuredValueId,
        UpdateMeasuredValueCommand command,
        [FromServices] MeasuredValueServices services,
        CancellationToken cancellationToken = default)
    {
        var result = await services.Commands.UpdateAsync(studyId, measuredValueId, command, cancellationToken);
        return result.IsFailure ? HandleFailure(result.GetError()) : Results.NoContent();
    }

    private static IResult HandleFailure(
        Exception error)
    {
        return error switch
        {
            KeyNotFoundException => Results.NotFound(new { error.Message }),
            ArgumentException or InvalidOperationException => Results.BadRequest(new { error.Message }),
            _ => Results.Problem(error.Message, statusCode: StatusCodes.Status500InternalServerError,
                title: "An unexpected error occurred")
        };
    }
}
