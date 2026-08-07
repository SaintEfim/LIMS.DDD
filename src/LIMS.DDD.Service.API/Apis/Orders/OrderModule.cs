using Carter;
using LIMS.DDD.Service.Application.Orders;
using LIMS.DDD.Service.Application.Orders.Commands;

namespace LIMS.DDD.Service.API.Apis.Orders;

public class OrderModule : ICarterModule
{
    public void AddRoutes(
        IEndpointRouteBuilder app)
    {
        var group = app.MapGroup("/api/orders")
            .WithTags("Orders");

        group.MapGet("/", GetAll)
            .Produces<ICollection<OrderDto>>();

        group.MapGet("/{id:guid}", GetById)
            .Produces<OrderDto>()
            .Produces(StatusCodes.Status404NotFound);

        group.MapPost("/", Create)
            .Produces(StatusCodes.Status201Created)
            .Produces(StatusCodes.Status400BadRequest)
            .Produces(StatusCodes.Status500InternalServerError);

        group.MapPatch("/{id:guid}", Update)
            .Produces(StatusCodes.Status204NoContent)
            .Produces(StatusCodes.Status404NotFound)
            .Produces(StatusCodes.Status400BadRequest);

        group.MapPost("/{id:guid}/change-status", ChangeStatus)
            .Produces(StatusCodes.Status204NoContent)
            .Produces(StatusCodes.Status404NotFound)
            .Produces(StatusCodes.Status400BadRequest);

        group.MapDelete("/{id:guid}", Delete)
            .Produces(StatusCodes.Status204NoContent)
            .Produces(StatusCodes.Status404NotFound)
            .Produces(StatusCodes.Status400BadRequest);
    }

    private static async Task<IResult> GetAll(
        [AsParameters] OrderServices services,
        CancellationToken ct)
    {
        var orders = await services.Queries.GetAllAsync(ct);
        return Results.Ok(orders);
    }

    private static async Task<IResult> GetById(
        Guid id,
        [AsParameters] OrderServices services,
        CancellationToken ct)
    {
        var dto = await services.Queries.GetByIdAsync(id, ct);
        return dto is not null ? Results.Ok(dto) : Results.NotFound();
    }

    private static async Task<IResult> Create(
        CreateOrderCommand command,
        [AsParameters] OrderServices services,
        CancellationToken ct)
    {
        var result = await services.Commands.CreateAsync(command, ct);

        if (result.IsFailure)
        {
            return HandleFailure(result.GetError());
        }

        var createdId = result.GetValue()
            .Id.Value;
        return Results.Created($"/api/orders/{createdId}", new { id = createdId });
    }

    private static async Task<IResult> Update(
        Guid id,
        UpdateOrderCommand command,
        [AsParameters] OrderServices services,
        CancellationToken ct)
    {
        var result = await services.Commands.UpdateAsync(id, command, ct);
        return result.IsFailure ? HandleFailure(result.GetError()) : Results.NoContent();
    }

    private static async Task<IResult> ChangeStatus(
        Guid id,
        ChangeOrderStatusCommand command,
        [AsParameters] OrderServices services,
        CancellationToken ct)
    {
        var result = await services.Commands.ChangeStatusAsync(id, command, ct);
        return result.IsFailure ? HandleFailure(result.GetError()) : Results.NoContent();
    }

    private static async Task<IResult> Delete(
        Guid id,
        [AsParameters] OrderServices services,
        CancellationToken ct)
    {
        var result = await services.Commands.DeleteAsync(id, ct);
        return result.IsFailure ? HandleFailure(result.GetError()) : Results.NoContent();
    }

    private static IResult HandleFailure(
        Exception error) =>
        error switch
        {
            KeyNotFoundException => Results.NotFound(new { error.Message }),
            ArgumentException or InvalidOperationException => Results.BadRequest(new { error.Message }),
            _ => Results.Problem(error.Message, statusCode: StatusCodes.Status500InternalServerError,
                title: "An unexpected error occurred")
        };
}
