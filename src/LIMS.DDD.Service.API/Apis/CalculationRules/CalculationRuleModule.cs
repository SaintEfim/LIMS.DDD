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

        var inputsGroup = group.MapGroup("/{ruleId:guid}/inputs")
            .WithTags("CalculationRuleInputs");

        inputsGroup.MapPost("/", CreateInput)
            .Produces(StatusCodes.Status201Created)
            .Produces(StatusCodes.Status404NotFound)
            .Produces(StatusCodes.Status400BadRequest)
            .Produces(StatusCodes.Status500InternalServerError);

        inputsGroup.MapDelete("/{variableAlias}", RemoveInput)
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
        [AsParameters] CalculationRuleServices services,
        CancellationToken ct)
    {
        var results = await services.Queries.GetAllByTemplateIdAsync(studyTemplateId, ct);
        return Results.Ok(results);
    }

    private static async Task<IResult> GetById(
        Guid studyTemplateId,
        Guid ruleId,
        [AsParameters] CalculationRuleServices services,
        CancellationToken ct)
    {
        var result = await services.Queries.GetByIdAsync(studyTemplateId, ruleId, ct);
        return result is not null ? Results.Ok(result) : Results.NotFound();
    }

    private static async Task<IResult> Create(
        Guid studyTemplateId,
        CreateCalculationRuleCommand command,
        [AsParameters] CalculationRuleServices services,
        CancellationToken ct)
    {
        var result = await services.Commands.CreateAsync(studyTemplateId, command, ct);

        if (result.IsFailure) return HandleFailure(result.Error!);

        var ruleId = result.Value;
        return Results.Created($"/api/studyTemplates/{studyTemplateId}/calculation-rules/{ruleId}",
            new { id = ruleId });
    }

    private static async Task<IResult> Delete(
        Guid studyTemplateId,
        Guid ruleId,
        [AsParameters] CalculationRuleServices services,
        CancellationToken ct)
    {
        var result = await services.Commands.RemoveAsync(studyTemplateId, ruleId, ct);
        return result.IsFailure ? HandleFailure(result.Error!) : Results.NoContent();
    }

    private static async Task<IResult> CreateInput(
        Guid studyTemplateId,
        Guid ruleId,
        AddCalculationInputCommand command,
        [AsParameters] CalculationRuleServices services,
        CancellationToken ct)
    {
        var result = await services.Commands.CreateInputAsync(studyTemplateId, ruleId, command, ct);
        return result.IsFailure ? HandleFailure(result.Error!) : Results.Created();
    }

    private static async Task<IResult> RemoveInput(
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

    private static async Task<IResult> Update(
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
