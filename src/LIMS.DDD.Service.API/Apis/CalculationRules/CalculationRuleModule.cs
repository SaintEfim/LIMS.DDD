using Carter;
using LIMS.DDD.Service.Application.StudyTemplates.CalculationRules.Commands;
using LIMS.DDD.Service.Application.StudyTemplates.CalculationRules.Queries;

namespace LIMS.DDD.Service.API.Apis.CalculationRules;

public class CalculationRuleModule : ICarterModule
{
    public void AddRoutes(
        IEndpointRouteBuilder app)
    {
        var group = app.MapGroup("/api/study-templates/{studyTemplateId:guid}/calculation-rules")
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

        var inputsGroup = group.MapGroup("/{ruleId:guid}/inputs")
            .WithTags("CalculationRuleInputs");

        inputsGroup.MapPost("/", AddCalculationInput)
            .Produces(StatusCodes.Status201Created)
            .Produces(StatusCodes.Status404NotFound)
            .Produces(StatusCodes.Status400BadRequest)
            .Produces(StatusCodes.Status500InternalServerError);

        inputsGroup.MapDelete("/{variableAlias}", RemoveCalculationInput)
            .Produces(StatusCodes.Status204NoContent)
            .Produces(StatusCodes.Status404NotFound)
            .Produces(StatusCodes.Status400BadRequest)
            .Produces(StatusCodes.Status500InternalServerError);

        group.MapPatch("/{ruleId:guid}", UpdateCalculation)
            .Produces(StatusCodes.Status204NoContent)
            .Produces(StatusCodes.Status404NotFound)
            .Produces(StatusCodes.Status400BadRequest);
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
        var result = await services.Commands.AddAsync(studyTemplateId, command, ct);

        if (result.IsFailure) return HandleFailure(result.Error!);

        var ruleId = result.Value;
        return Results.Created($"/api/studyTemplates/{studyTemplateId}/calculation-rules/{ruleId}",
            new { id = ruleId });
    }

    private static async Task<IResult> DeleteCalculation(
        Guid studyTemplateId,
        Guid ruleId,
        [AsParameters] CalculationRuleServices services,
        CancellationToken ct)
    {
        var result = await services.Commands.RemoveAsync(studyTemplateId, ruleId, ct);
        return result.IsFailure ? HandleFailure(result.Error!) : Results.NoContent();
    }

    private static async Task<IResult> AddCalculationInput(
        Guid studyTemplateId,
        Guid ruleId,
        AddCalculationInputCommand command,
        [AsParameters] CalculationRuleServices services,
        CancellationToken ct)
    {
        var result = await services.Commands.AddInputAsync(studyTemplateId, ruleId, command, ct);
        return result.IsFailure ? HandleFailure(result.Error!) : Results.Created();
    }

    private static async Task<IResult> RemoveCalculationInput(
        Guid studyTemplateId,
        Guid ruleId,
        string variableAlias,
        [AsParameters] CalculationRuleServices services,
        CancellationToken ct)
    {
        var command = new RemoveCalculationInputCommand(variableAlias);
        var result = await services.Commands.RemoveInputAsync(studyTemplateId, ruleId, command, ct);
        return result.IsFailure ? HandleFailure(result.Error!) : Results.NoContent();
    }

    private static async Task<IResult> UpdateCalculation(
        Guid studyTemplateId,
        Guid ruleId,
        UpdateCalculationRuleCommand command,
        [AsParameters] CalculationRuleServices services,
        CancellationToken ct)
    {
        var result = await services.Commands.UpdateAsync(studyTemplateId, ruleId, command, ct);
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
