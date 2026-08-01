using Carter;
using LIMS.DDD.Service.Application.StudyTemplates.ResultDefinitions.Commands;
using LIMS.DDD.Service.Application.StudyTemplates.ResultDefinitions.Queries;

namespace LIMS.DDD.Service.API.Apis.ResultDefinitions;

public class ResultDefinitionModule : ICarterModule
{
    public void AddRoutes(
        IEndpointRouteBuilder app)
    {
        var group = app.MapGroup("/api/study-templates/{studyTemplateId:guid}/result-definitions")
            .WithTags("ResultDefinitions");

        group.MapGet("/", GetAll)
            .Produces<ICollection<ResultDefinitionDto>>();

        group.MapGet("/{determinationId:guid}", GetById)
            .Produces<ResultDefinitionDto>()
            .Produces(StatusCodes.Status404NotFound);

        group.MapPost("/", Create)
            .Produces(StatusCodes.Status201Created)
            .Produces(StatusCodes.Status404NotFound)
            .Produces(StatusCodes.Status400BadRequest)
            .Produces(StatusCodes.Status500InternalServerError);

        group.MapDelete("/{determinationId:guid}", Delete)
            .Produces(StatusCodes.Status204NoContent)
            .Produces(StatusCodes.Status404NotFound)
            .Produces(StatusCodes.Status400BadRequest)
            .Produces(StatusCodes.Status500InternalServerError);

        group.MapPatch("/{determinationId:guid}", Update)
            .Produces(StatusCodes.Status204NoContent)
            .Produces(StatusCodes.Status404NotFound)
            .Produces(StatusCodes.Status400BadRequest);
    }

    private static async Task<IResult> GetAll(
        Guid studyTemplateId,
        [AsParameters] ResultDefinitionServices services,
        CancellationToken ct)
    {
        var results = await services.Queries.GetAllByTemplateIdAsync(studyTemplateId, ct);
        return Results.Ok(results);
    }

    private static async Task<IResult> GetById(
        Guid studyTemplateId,
        Guid resultId,
        [AsParameters] ResultDefinitionServices services,
        CancellationToken ct)
    {
        var result = await services.Queries.GetByIdAsync(studyTemplateId, resultId, ct);
        return result is not null ? Results.Ok(result) : Results.NotFound();
    }

    private static async Task<IResult> Create(
        Guid studyTemplateId,
        CreateResultDefinitionCommand command,
        [AsParameters] ResultDefinitionServices services,
        CancellationToken ct)
    {
        var result = await services.Commands.CreateAsync(studyTemplateId, command, ct);

        if (result.IsFailure) return HandleFailure(result.Error!);

        var resultId = result.Value;
        return Results.Created($"/api/studyTemplates/{studyTemplateId}/results/{resultId}", new { id = resultId });
    }

    private static async Task<IResult> Delete(
        Guid studyTemplateId,
        Guid resultId,
        [AsParameters] ResultDefinitionServices services,
        CancellationToken ct)
    {
        var result = await services.Commands.RemoveAsync(studyTemplateId, resultId, ct);

        return result.IsFailure ? HandleFailure(result.Error!) : Results.NoContent();
    }

    private static async Task<IResult> Update(
        Guid studyTemplateId,
        Guid determinationId,
        UpdateResultDefinitionCommand command,
        [AsParameters] ResultDefinitionServices services,
        CancellationToken ct)
    {
        var result = await services.Commands.UpdateAsync(
            studyTemplateId, determinationId, command, ct);
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
