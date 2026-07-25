using Carter;
using LIMS.DDD.Service.Application.StudyTemplates.StudyTemplateDeterminations.Commands;
using LIMS.DDD.Service.Application.StudyTemplates.StudyTemplateDeterminations.Queries;

namespace LIMS.DDD.Service.API.Apis.StudyTemplateDeterminations;

public class StudyTemplateDeterminationModule : ICarterModule
{
    public void AddRoutes(
        IEndpointRouteBuilder app)
    {
        var group = app.MapGroup("/api/studyTemplates/{studyTemplateId:guid}/determinations")
            .WithTags("StudyTemplateDeterminations");

        group.MapGet("/", GetAllDeterminations)
            .Produces<ICollection<StudyTemplateDeterminationDto>>();

        group.MapGet("/{determinationId:guid}", GetDeterminationById)
            .Produces<StudyTemplateDeterminationDto>()
            .Produces(StatusCodes.Status404NotFound);

        group.MapPost("/", CreateDetermination)
            .Produces(StatusCodes.Status201Created)
            .Produces(StatusCodes.Status404NotFound)
            .Produces(StatusCodes.Status400BadRequest)
            .Produces(StatusCodes.Status500InternalServerError);

        group.MapDelete("/{determinationId:guid}", DeleteDetermination)
            .Produces(StatusCodes.Status204NoContent)
            .Produces(StatusCodes.Status404NotFound)
            .Produces(StatusCodes.Status400BadRequest)
            .Produces(StatusCodes.Status500InternalServerError);
    }

    private static async Task<IResult> GetAllDeterminations(
        Guid studyTemplateId,
        [AsParameters] StudyTemplateDeterminationServices services,
        CancellationToken ct)
    {
        var results = await services.Queries.GetAllByTemplateIdAsync(studyTemplateId, ct);
        return Results.Ok(results);
    }

    private static async Task<IResult> GetDeterminationById(
        Guid studyTemplateId,
        Guid resultId,
        [AsParameters] StudyTemplateDeterminationServices services,
        CancellationToken ct)
    {
        var result = await services.Queries.GetByIdAsync(studyTemplateId, resultId, ct);
        return result is not null ? Results.Ok(result) : Results.NotFound();
    }

    private static async Task<IResult> CreateDetermination(
        Guid studyTemplateId,
        CreateStudyTemplateDeterminationCommand command,
        [AsParameters] StudyTemplateDeterminationServices services,
        CancellationToken ct)
    {
        var result = await services.Commands.AddStudyTemplateDeterminationAsync(studyTemplateId, command, ct);

        if (result.IsFailure) return HandleFailure(result.Error!);

        var resultId = result.Value;
        return Results.Created($"/api/studyTemplates/{studyTemplateId}/results/{resultId}", new { id = resultId });
    }

    private static async Task<IResult> DeleteDetermination(
        Guid studyTemplateId,
        Guid resultId,
        [AsParameters] StudyTemplateDeterminationServices services,
        CancellationToken ct)
    {
        var result = await services.Commands.RemoveStudyTemplateDeterminationAsync(studyTemplateId, resultId, ct);

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
