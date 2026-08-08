using Carter;
using LIMS.DDD.Service.Application.Samples;
using LIMS.DDD.Service.Application.Samples.Commands;
using LIMS.DDD.Service.Domain.LaboratoryOperationsContext.OrderAggregate;

namespace LIMS.DDD.Service.API.Apis.Samples;

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
        [AsParameters] SampleServices services,
        CancellationToken ct)
    {
        var samples = await services.Queries.GetAllByOrderIdAsync(orderId, ct);
        return Results.Ok(samples);
    }

    private static async Task<IResult> GetById(
        Guid orderId,
        Guid sampleId,
        [AsParameters] SampleServices services,
        CancellationToken ct)
    {
        var dto = await services.Queries.GetByIdAsync(sampleId, ct);
        return dto is not null ? Results.Ok(dto) : Results.NotFound();
    }

    private static async Task<IResult> Create(
        Guid orderId,
        CreateSampleCommand command,
        [AsParameters] SampleServices services,
        CancellationToken ct)
    {
        var result = await services.Commands.CreateAsync(new OrderId(orderId), command, ct);

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
        [AsParameters] SampleServices services,
        CancellationToken ct)
    {
        var result = await services.Commands.UpdateAsync(sampleId, command, ct);
        return result.IsFailure ? HandleFailure(result.GetError()) : Results.NoContent();
    }

    private static async Task<IResult> ChangeStatus(
        Guid orderId,
        Guid sampleId,
        string newStatus,
        [AsParameters] SampleServices services,
        CancellationToken ct)
    {
        var result = await services.Commands.ChangeStatusAsync(sampleId, newStatus, ct);
        return result.IsFailure ? HandleFailure(result.GetError()) : Results.Ok();
    }

    private static async Task<IResult> Delete(
        Guid orderId,
        Guid sampleId,
        [AsParameters] SampleServices services,
        CancellationToken ct)
    {
        var result = await services.Commands.DeleteAsync(sampleId, ct);
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
