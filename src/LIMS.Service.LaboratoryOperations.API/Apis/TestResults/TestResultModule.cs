using Carter;
using LIMS.Service.LaboratoryOperations.Application.Studies.TestResults;
using Microsoft.AspNetCore.Mvc;

namespace LIMS.Service.LaboratoryOperations.API.Apis.TestResults;

public class TestResultModule : ICarterModule
{
    public void AddRoutes(
        IEndpointRouteBuilder app)
    {
        var group = app.MapGroup("/api/studies/{studyId:guid}/test-results")
            .WithTags("TestResults");

        group.MapGet("/", GetAll)
            .Produces<ICollection<TestResultDto>>();

        group.MapGet("/{testResultId:guid}", GetById)
            .Produces<TestResultDto>()
            .Produces(StatusCodes.Status404NotFound);

        group.MapPatch("/{testResultId:guid}", Update)
            .Produces(StatusCodes.Status204NoContent)
            .Produces(StatusCodes.Status404NotFound)
            .Produces(StatusCodes.Status400BadRequest);

        group.MapPost("/{testResultId:guid}/execute", Execute)
            .Produces(StatusCodes.Status204NoContent)
            .Produces(StatusCodes.Status404NotFound)
            .Produces(StatusCodes.Status400BadRequest);
    }

    private static async Task<IResult> Execute(
        Guid studyId,
        Guid testResultId,
        [FromServices] TestResultServices services,
        CancellationToken cancellationToken = default)
    {
        var result = await services.Commands.ExecuteTest(studyId, testResultId, cancellationToken);
        return result.IsFailure ? HandleFailure(result.GetError()) : Results.NoContent();
    }

    private static async Task<IResult> Update(
        Guid studyId,
        Guid testResultId,
        UpdateTestResultCommand command,
        [FromServices] TestResultServices services,
        CancellationToken cancellationToken = default)
    {
        var result = await services.Commands.UpdateAsync(studyId, testResultId, command, cancellationToken);
        return result.IsFailure ? HandleFailure(result.GetError()) : Results.NoContent();
    }

    private static async Task<IResult> GetAll(
        Guid studyId,
        [FromServices] TestResultServices services,
        CancellationToken cancellationToken = default)
    {
        var results = await services.Queries.GetAllByStudyIdAsync(studyId, cancellationToken);
        return Results.Ok(results);
    }

    private static async Task<IResult> GetById(
        Guid studyId,
        Guid testResultId,
        [FromServices] TestResultServices services,
        CancellationToken cancellationToken = default)
    {
        var dto = await services.Queries.GetByIdAsync(studyId, testResultId, cancellationToken);
        return dto is not null ? Results.Ok(dto) : Results.NotFound();
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
