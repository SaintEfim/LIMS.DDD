using Carter;
using LIMS.DDD.Service.Application.Studies.TestResults;

namespace LIMS.DDD.Service.API.Apis.TestResults;

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

        group.MapPost("/{testResultId:guid}", Execute)
            .Produces(StatusCodes.Status204NoContent)
            .Produces(StatusCodes.Status404NotFound)
            .Produces(StatusCodes.Status400BadRequest);
    }

    private static async Task<IResult> Execute(
        Guid studyId,
        Guid testResultId,
        [AsParameters] TestResultServices services,
        CancellationToken ct)
    {
        var result = await services.Commands.ExecuteTest(studyId, testResultId, ct);
        return result.IsFailure ? HandleFailure(result.GetError()) : Results.NoContent();
    }

    private static async Task<IResult> GetAll(
        Guid studyId,
        [AsParameters] TestResultServices services,
        CancellationToken ct)
    {
        var results = await services.Queries.GetAllByStudyIdAsync(studyId, ct);
        return Results.Ok(results);
    }

    private static async Task<IResult> GetById(
        Guid studyId,
        Guid testResultId,
        [AsParameters] TestResultServices services,
        CancellationToken ct)
    {
        var dto = await services.Queries.GetByIdAsync(studyId, testResultId, ct);
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
