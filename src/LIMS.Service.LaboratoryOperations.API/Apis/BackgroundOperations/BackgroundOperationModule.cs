using System.Security.Claims;
using Carter;
using LIMS.Service.LaboratoryOperations.Application.BackgroundOperations;
using LIMS.Service.LaboratoryOperations.Domain.BackgroundOperations;
using Microsoft.AspNetCore.Mvc;

namespace LIMS.Service.LaboratoryOperations.API.Apis.BackgroundOperations;

public sealed class BackgroundOperationModule : ICarterModule
{
    private static readonly TimeSpan WaitTimeout = TimeSpan.FromSeconds(25);
    private static readonly TimeSpan CheckInterval = TimeSpan.FromSeconds(1);

    public void AddRoutes(
        IEndpointRouteBuilder app)
    {
        var group = app.MapGroup("/api/background-operations")
            .WithTags("Background operations")
            .RequireAuthorization();

        group.MapGet("/{id:guid}/wait", WaitAsync)
            .Produces<BackgroundOperationStatusDto>()
            .Produces(StatusCodes.Status204NoContent)
            .Produces(StatusCodes.Status404NotFound)
            .Produces(StatusCodes.Status403Forbidden);
    }

    private static async Task<IResult> WaitAsync(
        Guid id,
        [FromServices] ClaimsPrincipal user,
        [FromServices] BackgroundOperationQueries queries,
        [FromServices] HttpContext context,
        CancellationToken cancellationToken = default)
    {
        context.Response.Headers.CacheControl = "no-store";
        if (!Guid.TryParse(user.FindFirstValue("user_id"), out var userId)) return Results.Forbid();

        using var timeout = CancellationTokenSource.CreateLinkedTokenSource(cancellationToken);
        timeout.CancelAfter(WaitTimeout);
        try
        {
            while (true)
            {
                var operation = await queries.GetByIdAsync(id, timeout.Token);
                if (operation is null || operation.RequestedByUserId != userId) return Results.NotFound();
                if (operation.Status is OperationStatus.Succeeded or OperationStatus.Failed or OperationStatus.Canceled)
                    return Results.Ok(BackgroundOperationStatusDto.FromOperation(operation));

                await Task.Delay(CheckInterval, timeout.Token);
            }
        }
        catch (OperationCanceledException) when (timeout.IsCancellationRequested &&
                                                 !cancellationToken.IsCancellationRequested)
        {
            return Results.NoContent();
        }
    }
}
