using Carter;
using LIMS.Service.LaboratoryOperations.Application.Samples;
using LIMS.Service.LaboratoryOperations.Application.Samples.Commands;
using LIMS.Service.LaboratoryOperations.Domain.OrderAggregate;
using Microsoft.AspNetCore.Mvc;

namespace LIMS.Service.LaboratoryOperations.API.Apis.Samples;

public class SampleModule : ICarterModule
{
    public void AddRoutes(
        IEndpointRouteBuilder app)
    {
        var group = app.MapGroup("/api/orders/{orderId:guid}/samples")
            .WithTags("Samples");

        group.MapGet("/", GetAll)
            .Produces<ICollection<SampleDto>>();

        group.MapGet("/{sampleId:guid}", GetById)
            .Produces<SampleDto>()
            .Produces(StatusCodes.Status404NotFound);

        group.MapPost("/", Create)
            .Produces(StatusCodes.Status201Created)
            .Produces(StatusCodes.Status404NotFound)
            .Produces(StatusCodes.Status400BadRequest)
            .Produces(StatusCodes.Status500InternalServerError);

        group.MapPatch("/{sampleId:guid}", Update)
            .Produces(StatusCodes.Status204NoContent)
            .Produces(StatusCodes.Status404NotFound)
            .Produces(StatusCodes.Status400BadRequest);

        group.MapPost("/{sampleId:guid}/change-status", ChangeStatus)
            .Produces(StatusCodes.Status200OK)
            .Produces(StatusCodes.Status404NotFound)
            .Produces(StatusCodes.Status400BadRequest);

        group.MapDelete("/{sampleId:guid}", Delete)
            .Produces(StatusCodes.Status204NoContent)
            .Produces(StatusCodes.Status404NotFound)
            .Produces(StatusCodes.Status400BadRequest);
    }

    private static async Task<IResult> GetAll(
        Guid orderId,
        [FromServices] SampleServices services,
        CancellationToken cancellationToken = default)
    {
        var samples = await services.Queries.GetAllByOrderIdAsync(orderId, cancellationToken);
        return Results.Ok(samples);
    }

    private static async Task<IResult> GetById(
        Guid orderId,
        Guid sampleId,
        [FromServices] SampleServices services,
        CancellationToken cancellationToken = default)
    {
        var dto = await services.Queries.GetByIdAsync(sampleId, cancellationToken);
        return dto is not null ? Results.Ok(dto) : Results.NotFound();
    }

    private static async Task<IResult> Create(
        Guid orderId,
        CreateSampleCommand command,
        [FromServices] SampleServices services,
        CancellationToken cancellationToken = default)
    {
        var result = await services.Commands.CreateAsync(new OrderId(orderId), command, cancellationToken);

        if (result.IsFailure)
        {
            return HandleFailure(result.GetError());
        }

        var createdId = result.GetValue()
            .Id.Value;
        return Results.Created($"/api/orders/{orderId}/samples/{createdId}", new { id = createdId });
    }

    private static async Task<IResult> Update(
        Guid orderId,
        Guid sampleId,
        UpdateSampleCommand command,
        [FromServices] SampleServices services,
        CancellationToken cancellationToken = default)
    {
        var result = await services.Commands.UpdateAsync(sampleId, command, cancellationToken);
        return result.IsFailure ? HandleFailure(result.GetError()) : Results.NoContent();
    }

    private static async Task<IResult> ChangeStatus(
        Guid orderId,
        Guid sampleId,
        string newStatus,
        [FromServices] SampleServices services,
        CancellationToken cancellationToken = default)
    {
        var result = await services.Commands.ChangeStatusAsync(sampleId, newStatus, cancellationToken);
        return result.IsFailure ? HandleFailure(result.GetError()) : Results.Ok();
    }

    private static async Task<IResult> Delete(
        Guid orderId,
        Guid sampleId,
        [FromServices] SampleServices services,
        CancellationToken cancellationToken = default)
    {
        var result = await services.Commands.DeleteAsync(sampleId, cancellationToken);
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
