using Carter;
using LIMS.Service.Methodologies.Application.StudyTemplates.CalculationRules;
using LIMS.Service.Methodologies.Application.StudyTemplates.CalculationRules.Commands;
using Microsoft.AspNetCore.Mvc;

namespace LIMS.Service.Methodologies.API.Apis.CalculationRules;

public class CalculationRuleModule : ICarterModule
{
    public void AddRoutes(
        IEndpointRouteBuilder app)
    {
        var group = app.MapGroup("/api/study-templates/{studyTemplateId:guid}/calculation-rules")
            .WithTags("CalculationRules");

        group.MapGet("/", GetAll)
            .Produces<ICollection<CalculationRuleDto>>();

        group.MapGet("/{ruleId:guid}", GetById)
            .Produces<CalculationRuleDto>()
            .Produces(StatusCodes.Status404NotFound);

        group.MapPost("/", Create)
            .Produces(StatusCodes.Status201Created)
            .Produces(StatusCodes.Status404NotFound)
            .Produces(StatusCodes.Status400BadRequest)
            .Produces(StatusCodes.Status500InternalServerError);

        group.MapDelete("/{ruleId:guid}", Delete)
            .Produces(StatusCodes.Status204NoContent)
            .Produces(StatusCodes.Status404NotFound)
            .Produces(StatusCodes.Status400BadRequest)
            .Produces(StatusCodes.Status500InternalServerError);

        group.MapPatch("/{ruleId:guid}", Update)
            .Produces(StatusCodes.Status204NoContent)
            .Produces(StatusCodes.Status404NotFound)
            .Produces(StatusCodes.Status400BadRequest);
    }

    private static async Task<IResult> GetAll(
        Guid studyTemplateId,
        [FromServices] CalculationRuleServices services,
        CancellationToken cancellationToken = default)
    {
        var results = await services.Queries.GetAllByTemplateIdAsync(studyTemplateId, cancellationToken);
        return Results.Ok(results);
    }

    private static async Task<IResult> GetById(
        Guid studyTemplateId,
        Guid ruleId,
        [FromServices] CalculationRuleServices services,
        CancellationToken cancellationToken = default)
    {
        var result = await services.Queries.GetByIdAsync(studyTemplateId, ruleId, cancellationToken);
        return result is not null ? Results.Ok(result) : Results.NotFound();
    }

    private static async Task<IResult> Create(
        Guid studyTemplateId,
        CreateCalculationRuleCommand command,
        [FromServices] CalculationRuleServices services,
        CancellationToken cancellationToken = default)
    {
        var result = await services.Commands.CreateAsync(studyTemplateId, command, cancellationToken);

        if (result.IsFailure)
        {
            return HandleFailure(result.GetError());
        }

        var ruleId = result.GetValue();
        return Results.Created($"/api/studyTemplates/{studyTemplateId}/calculation-rules/{ruleId}",
            new { id = ruleId });
    }

    private static async Task<IResult> Delete(
        Guid studyTemplateId,
        Guid ruleId,
        [FromServices] CalculationRuleServices services,
        CancellationToken cancellationToken = default)
    {
        var result = await services.Commands.RemoveAsync(studyTemplateId, ruleId, cancellationToken);
        return result.IsFailure ? HandleFailure(result.GetError()) : Results.NoContent();
    }

    private static async Task<IResult> Update(
        Guid studyTemplateId,
        Guid ruleId,
        UpdateCalculationRuleCommand command,
        [FromServices] CalculationRuleServices services,
        CancellationToken cancellationToken = default)
    {
        var result = await services.Commands.UpdateAsync(studyTemplateId, ruleId, command, cancellationToken);
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
