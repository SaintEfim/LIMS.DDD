using Carter;
using LIMS.DDD.Service.Application.StudyTemplates.ResultDefinitions.Commands;
using LIMS.DDD.Service.Application.StudyTemplates.ResultDefinitions.Queries;

namespace LIMS.DDD.Service.API.Apis.ResultDefinitions;

public class ResultDefinitionModule : ICarterModule
{
    public void AddRoutes(
        IEndpointRouteBuilder app)
    {
        var group = app.MapGroup("/api/studyTemplates/{studyTemplateId:guid}/determinations")
            .WithTags("ResultDefinitions");

        group.MapGet("/", GetAllResultDefinitions)
            .Produces<ICollection<ResultDefinitionDto>>();

        group.MapGet("/{determinationId:guid}", GetResultDefinitionById)
            .Produces<ResultDefinitionDto>()
            .Produces(StatusCodes.Status404NotFound);

        group.MapPost("/", CreateResultDefinition)
            .Produces(StatusCodes.Status201Created)
            .Produces(StatusCodes.Status404NotFound)
            .Produces(StatusCodes.Status400BadRequest)
            .Produces(StatusCodes.Status500InternalServerError);

        group.MapDelete("/{determinationId:guid}", DeleteResultDefinition)
            .Produces(StatusCodes.Status204NoContent)
            .Produces(StatusCodes.Status404NotFound)
            .Produces(StatusCodes.Status400BadRequest)
            .Produces(StatusCodes.Status500InternalServerError);
    }

    private static async Task<IResult> GetAllResultDefinitions(
        Guid studyTemplateId,
        [AsParameters] ResultDefinitionServices services,
        CancellationToken ct)
    {
        var results = await services.Queries.GetAllByTemplateIdAsync(studyTemplateId, ct);
        return Results.Ok(results);
    }

    private static async Task<IResult> GetResultDefinitionById(
        Guid studyTemplateId,
        Guid resultId,
        [AsParameters] ResultDefinitionServices services,
        CancellationToken ct)
    {
        var result = await services.Queries.GetByIdAsync(studyTemplateId, resultId, ct);
        return result is not null ? Results.Ok(result) : Results.NotFound();
    }

    private static async Task<IResult> CreateResultDefinition(
        Guid studyTemplateId,
        CreateResultDefinitionCommand command,
        [AsParameters] ResultDefinitionServices services,
        CancellationToken ct)
    {
        var result = await services.Commands.AddResultDefinitionAsync(studyTemplateId, command, ct);

        if (result.IsFailure) return HandleFailure(result.Error!);

        var resultId = result.Value;
        return Results.Created($"/api/studyTemplates/{studyTemplateId}/results/{resultId}", new { id = resultId });
    }

    private static async Task<IResult> DeleteResultDefinition(
        Guid studyTemplateId,
        Guid resultId,
        [AsParameters] ResultDefinitionServices services,
        CancellationToken ct)
    {
        var result = await services.Commands.RemoveResultDefinitionAsync(studyTemplateId, resultId, ct);

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
