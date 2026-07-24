using Carter;
using LIMS.DDD.Service.Application.StudyTemplates.StudyTemplateResults.Commands;
using LIMS.DDD.Service.Application.StudyTemplates.StudyTemplateResults.Queries;

namespace LIMS.DDD.Service.API.Apis.StudyTemplateResults;

public class StudyTemplateResultModule : ICarterModule
{
    public void AddRoutes(
        IEndpointRouteBuilder app)
    {
        var group = app.MapGroup("/api/studyTemplates/{studyTemplateId:guid}/results")
            .WithTags("StudyTemplateResults");

        group.MapGet("/", GetAllResults)
            .Produces<ICollection<StudyTemplateResultDto>>();

        group.MapGet("/{resultId:guid}", GetResultById)
            .Produces<StudyTemplateResultDto>()
            .Produces(StatusCodes.Status404NotFound);

        group.MapPost("/", CreateResult)
            .Produces(StatusCodes.Status201Created)
            .Produces(StatusCodes.Status404NotFound)
            .Produces(StatusCodes.Status400BadRequest)
            .Produces(StatusCodes.Status500InternalServerError);

        group.MapDelete("/{resultId:guid}", DeleteResult)
            .Produces(StatusCodes.Status204NoContent)
            .Produces(StatusCodes.Status404NotFound)
            .Produces(StatusCodes.Status400BadRequest)
            .Produces(StatusCodes.Status500InternalServerError);
    }

    private static async Task<IResult> GetAllResults(
        Guid studyTemplateId,
        [AsParameters] StudyTemplateResultServices services,
        CancellationToken ct)
    {
        var results = await services.Queries.GetAllByTemplateIdAsync(studyTemplateId, ct);
        return Results.Ok(results);
    }

    private static async Task<IResult> GetResultById(
        Guid studyTemplateId,
        Guid resultId,
        [AsParameters] StudyTemplateResultServices services,
        CancellationToken ct)
    {
        var result = await services.Queries.GetByIdAsync(studyTemplateId, resultId, ct);
        return result is not null ? Results.Ok(result) : Results.NotFound();
    }

    private static async Task<IResult> CreateResult(
        Guid studyTemplateId,
        CreateStudyTemplateResultCommand command,
        [AsParameters] StudyTemplateResultServices services,
        CancellationToken ct)
    {
        var result = await services.Commands.AddStudyTemplateResultAsync(studyTemplateId, command, ct);

        if (result.IsFailure) return HandleFailure(result.Error!);

        var resultId = result.Value;
        return Results.Created($"/api/studyTemplates/{studyTemplateId}/results/{resultId}", new { id = resultId });
    }

    private static async Task<IResult> DeleteResult(
        Guid studyTemplateId,
        Guid resultId,
        [AsParameters] StudyTemplateResultServices services,
        CancellationToken ct)
    {
        var result = await services.Commands.RemoveStudyTemplateResultAsync(studyTemplateId, resultId, ct);

        return result.IsFailure ? HandleFailure(result.Error!) : Results.NoContent();
    }

    private static IResult HandleFailure(
        Exception error) =>
        error switch
        {
            KeyNotFoundException => Results.NotFound(new { error.Message }),
            InvalidOperationException => Results.BadRequest(new { error.Message }),
            _ => Results.Problem(detail: error.Message, statusCode: StatusCodes.Status500InternalServerError,
                title: "An unexpected error occurred")
        };
}
