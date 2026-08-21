using Carter;
using LIMS.Service.Methodologies.Application.StudyTemplates.ResultDefinitions;
using LIMS.Service.Methodologies.Application.StudyTemplates.ResultDefinitions.Commands;
using Microsoft.AspNetCore.Mvc;

namespace LIMS.Service.Methodologies.API.Apis.ResultDefinitions;

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
        [FromServices] ResultDefinitionServices services,
        CancellationToken cancellationToken = default)
    {
        var results = await services.Queries.GetAllByTemplateIdAsync(studyTemplateId, cancellationToken);
        return Results.Ok(results);
    }

    private static async Task<IResult> GetById(
        Guid studyTemplateId,
        Guid resultId,
        [FromServices] ResultDefinitionServices services,
        CancellationToken cancellationToken = default)
    {
        var result = await services.Queries.GetByIdAsync(studyTemplateId, resultId, cancellationToken);
        return result is not null ? Results.Ok(result) : Results.NotFound();
    }

    private static async Task<IResult> Create(
        Guid studyTemplateId,
        CreateResultDefinitionCommand command,
        [FromServices] ResultDefinitionServices services,
        CancellationToken cancellationToken = default)
    {
        var result = await services.Commands.CreateAsync(studyTemplateId, command, cancellationToken);

        if (result.IsFailure)
        {
            return HandleFailure(result.GetError());
        }

        var resultId = result.GetValue();
        return Results.Created($"/api/studyTemplates/{studyTemplateId}/results/{resultId}", new { id = resultId });
    }

    private static async Task<IResult> Delete(
        Guid studyTemplateId,
        Guid resultId,
        [FromServices] ResultDefinitionServices services,
        CancellationToken cancellationToken = default)
    {
        var result = await services.Commands.RemoveAsync(studyTemplateId, resultId, cancellationToken);

        return result.IsFailure ? HandleFailure(result.GetError()) : Results.NoContent();
    }

    private static async Task<IResult> Update(
        Guid studyTemplateId,
        Guid determinationId,
        UpdateResultDefinitionCommand command,
        [FromServices] ResultDefinitionServices services,
        CancellationToken cancellationToken = default)
    {
        var result = await services.Commands.UpdateAsync(studyTemplateId, determinationId, command, cancellationToken);
        return result.IsFailure ? HandleFailure(result.GetError()) : Results.NoContent();
    }

    private static IResult HandleFailure(
        Exception error)
    {
        return error switch
        {
            KeyNotFoundException => Results.NotFound(new { error.Message }),
            InvalidOperationException => Results.BadRequest(new { error.Message }),
            _ => Results.Problem(error.Message, statusCode: StatusCodes.Status500InternalServerError,
                title: "An unexpected error occurred")
        };
    }
}
