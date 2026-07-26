using Carter;
using LIMS.DDD.Service.Application.StudyTemplates.CalculationRules.Commands;
using LIMS.DDD.Service.Application.StudyTemplates.CalculationRules.Queries;

namespace LIMS.DDD.Service.API.Apis.CalculationRules;

public class CalculationRuleModule : ICarterModule
{
    public void AddRoutes(IEndpointRouteBuilder app)
    {
        var group = app.MapGroup("/api/studyTemplates/{studyTemplateId:guid}/calculations")
            .WithTags("CalculationRules");

        group.MapGet("/", GetAllCalculations)
            .Produces<ICollection<CalculationRuleDto>>();

        group.MapGet("/{ruleId:guid}", GetCalculationById)
            .Produces<CalculationRuleDto>()
            .Produces(StatusCodes.Status404NotFound);

        group.MapPost("/", CreateCalculation)
            .Produces(StatusCodes.Status201Created)
            .Produces(StatusCodes.Status404NotFound)
            .Produces(StatusCodes.Status400BadRequest)
            .Produces(StatusCodes.Status500InternalServerError);

        group.MapDelete("/{ruleId:guid}", DeleteCalculation)
            .Produces(StatusCodes.Status204NoContent)
            .Produces(StatusCodes.Status404NotFound)
            .Produces(StatusCodes.Status400BadRequest)
            .Produces(StatusCodes.Status500InternalServerError);
    }

    private static async Task<IResult> GetAllCalculations(
        Guid studyTemplateId,
        [AsParameters] CalculationRuleServices services,
        CancellationToken ct)
    {
        var results = await services.Queries.GetAllByTemplateIdAsync(studyTemplateId, ct);
        return Results.Ok(results);
    }

    private static async Task<IResult> GetCalculationById(
        Guid studyTemplateId,
        Guid ruleId,
        [AsParameters] CalculationRuleServices services,
        CancellationToken ct)
    {
        var result = await services.Queries.GetByIdAsync(studyTemplateId, ruleId, ct);
        return result is not null ? Results.Ok(result) : Results.NotFound();
    }

    private static async Task<IResult> CreateCalculation(
        Guid studyTemplateId,
        CreateCalculationRuleCommand command,
        [AsParameters] CalculationRuleServices services,
        CancellationToken ct)
    {
        var result = await services.Commands.AddCalculationRuleAsync(studyTemplateId, command, ct);

        if (result.IsFailure) return HandleFailure(result.Error!);

        var ruleId = result.Value;
        return Results.Created($"/api/studyTemplates/{studyTemplateId}/calculations/{ruleId}", new { id = ruleId });
    }

    private static async Task<IResult> DeleteCalculation(
        Guid studyTemplateId,
        Guid ruleId,
        [AsParameters] CalculationRuleServices services,
        CancellationToken ct)
    {
        var result = await services.Commands.RemoveCalculationRuleAsync(studyTemplateId, ruleId, ct);
        return result.IsFailure ? HandleFailure(result.Error!) : Results.NoContent();
    }

    private static IResult HandleFailure(Exception error) =>
        error switch
        {
            KeyNotFoundException => Results.NotFound(new { error.Message }),
            InvalidOperationException => Results.BadRequest(new { error.Message }),
            _ => Results.Problem(
                detail: error.Message,
                statusCode: StatusCodes.Status500InternalServerError,
                title: "An unexpected error occurred")
        };
}
