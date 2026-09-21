using System.Security.Claims;
using Carter;
using LIMS.Service.LaboratoryOperations.Application.BackgroundOperations;
using LIMS.Service.LaboratoryOperations.Application.Orders;
using LIMS.Service.LaboratoryOperations.Application.Orders.Commands;
using LIMS.Service.LaboratoryOperations.Application.Samples;
using LIMS.Service.LaboratoryOperations.Domain.BackgroundOperations;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json.Linq;
using Service.Reports.Client;
using Service.Reports.Client.Models;

namespace LIMS.Service.LaboratoryOperations.API.Apis.Orders;

public class OrderModule
    : ModuleBase,
        ICarterModule
{
    public void AddRoutes(
        IEndpointRouteBuilder app)
    {
        var group = app.MapGroup("/api/orders")
            .WithTags("Orders")
            .RequireAuthorization();

        group.MapGet("/", GetAll)
            .Produces<ICollection<OrderDto>>();

        group.MapGet("/{id:guid}", GetById)
            .Produces<OrderDto>()
            .Produces(StatusCodes.Status404NotFound);

        group.MapPost("/{id:guid}/report", GenerateReport)
            .Produces(StatusCodes.Status200OK, contentType: "application/pdf")
            .Produces(StatusCodes.Status404NotFound)
            .Produces(StatusCodes.Status502BadGateway);

        group.MapPost("/{id:guid}/report-async", GenerateReportAsync)
            .Produces(StatusCodes.Status202Accepted)
            .Produces(StatusCodes.Status400BadRequest)
            .Produces(StatusCodes.Status403Forbidden);

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
        [FromServices] OrderServices services,
        CancellationToken cancellationToken = default)
    {
        var orders = await services.Queries.GetAllAsync(cancellationToken);
        return Results.Ok(orders);
    }

    private static async Task<IResult> GetById(
        Guid id,
        [FromServices] OrderServices services,
        CancellationToken cancellationToken = default)
    {
        var dto = await services.Queries.GetByIdAsync(id, cancellationToken);
        return dto is not null ? Results.Ok(dto) : Results.NotFound();
    }

    private static async Task<IResult> GenerateReportAsync(
        Guid id,
        ClaimsPrincipal user,
        BackgroundOperationCommandsHandler commands,
        CancellationToken cancellationToken = default)
    {
        var subject = user.FindFirstValue("user_id");

        if (!Guid.TryParse(subject, out var userId))
        {
            return Results.Forbid();
        }

        var command = new CreateBackgroundOperationCommand(RequestedByUserId: userId,
            Type: OperationType.GenerateReport, Payload: JObject.FromObject(new ReportPayload(id)));

        var result = await commands.CreateAsync(command, cancellationToken);

        if (result.IsFailure)
        {
            return Results.BadRequest(result.GetError()
                .Message);
        }

        var operation = result.GetValue();

        return Results.Accepted($"/api/background-operations/{operation.Id}", new
        {
            operationId = operation.Id,
            status = OperationStatus.Pending
        });
    }

    private static async Task<IResult> GenerateReport(
        Guid id,
        [FromServices] OrderServices services,
        [FromServices] SampleQueries sampleQueries,
        [FromServices] IReportClient reportClient,
        CancellationToken cancellationToken = default)
    {
        var order = await services.Queries.GetByIdAsync(id, cancellationToken);
        if (order is null) return Results.NotFound();

        var samples = await sampleQueries.GetAllByOrderIdAsync(id, cancellationToken);
        try
        {
            var orderClient = new OrderInfo(order.Id, order.Name, order.Description, order.Code, order.Contractor,
                order.Status);

            var sampleList = samples.Select(x => new SampleInfo(x.Id, x.OrderId, x.Name, x.GatherDateBegin,
                    x.GatherDateEnd, x.Code, x.VolumeValue, x.VolumeUnit, x.Status))
                .ToList();

            var pdf = await reportClient.GenerateReportAsync(orderClient, sampleList, cancellationToken);

            return Results.File(pdf, "application/pdf", $"order-{id}.pdf");
        }
        catch (HttpRequestException)
        {
            return Results.Problem("Report service is unavailable.", statusCode: StatusCodes.Status502BadGateway);
        }
        catch (TaskCanceledException) when (!cancellationToken.IsCancellationRequested)
        {
            return Results.Problem("Report service timed out.", statusCode: StatusCodes.Status504GatewayTimeout);
        }
    }

    private static async Task<IResult> Create(
        CreateOrderCommand command,
        [FromServices] OrderServices services,
        CancellationToken cancellationToken = default)
    {
        var result = await services.Commands.CreateAsync(command, cancellationToken);

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
        [FromServices] OrderServices services,
        CancellationToken cancellationToken = default)
    {
        var result = await services.Commands.UpdateAsync(id, command, cancellationToken);
        return result.IsFailure ? HandleFailure(result.GetError()) : Results.NoContent();
    }

    private static async Task<IResult> ChangeStatus(
        Guid id,
        ChangeOrderStatusCommand command,
        [FromServices] OrderServices services,
        CancellationToken cancellationToken = default)
    {
        var result = await services.Commands.ChangeStatusAsync(id, command, cancellationToken);
        return result.IsFailure ? HandleFailure(result.GetError()) : Results.NoContent();
    }

    private static async Task<IResult> Delete(
        Guid id,
        [FromServices] OrderServices services,
        CancellationToken cancellationToken = default)
    {
        var result = await services.Commands.DeleteAsync(id, cancellationToken);
        return result.IsFailure ? HandleFailure(result.GetError()) : Results.NoContent();
    }
}
