using Carter;
using LIMS.DDD.Service.Application.StudyTemplates.InputParameters.Commands;
using LIMS.DDD.Service.Application.StudyTemplates.InputParameters.Queries;

namespace LIMS.DDD.Service.API.Apis.InputParameters;

public class InputParameterModule : ICarterModule
{
    public void AddRoutes(
        IEndpointRouteBuilder app)
    {
        var group = app.MapGroup("/api/study-templates/{studyTemplateId:guid}/input-parameters")
            .WithTags("InputParameters");

        group.MapGet("/", GetAll)
            .Produces<ICollection<InputParameterDto>>();

        group.MapGet("/{parameterId:guid}", GetById)
            .Produces<InputParameterDto>()
            .Produces(StatusCodes.Status404NotFound);

        group.MapPost("/", Create)
            .Produces(StatusCodes.Status201Created)
            .Produces(StatusCodes.Status404NotFound)
            .Produces(StatusCodes.Status400BadRequest)
            .Produces(StatusCodes.Status500InternalServerError);

        group.MapDelete("/{parameterId:guid}", Delete)
            .Produces(StatusCodes.Status204NoContent)
            .Produces(StatusCodes.Status404NotFound)
            .Produces(StatusCodes.Status400BadRequest)
            .Produces(StatusCodes.Status500InternalServerError);

        group.MapPatch("/{parameterId:guid}", Update)
            .Produces(StatusCodes.Status204NoContent)
            .Produces(StatusCodes.Status404NotFound)
            .Produces(StatusCodes.Status400BadRequest);
    }

    private static async Task<IResult> GetAll(
        Guid studyTemplateId,
        [AsParameters] InputParameterServices services,
        CancellationToken ct)
    {
        var parameters = await services.Queries.GetAllByTemplateIdAsync(studyTemplateId, ct);
        return Results.Ok(parameters);
    }

    private static async Task<IResult> GetById(
        Guid studyTemplateId,
        Guid parameterId,
        [AsParameters] InputParameterServices services,
        CancellationToken ct)
    {
        var parameter = await services.Queries.GetByIdAsync(studyTemplateId, parameterId, ct);
        return parameter is not null ? Results.Ok(parameter) : Results.NotFound();
    }

    private static async Task<IResult> Create(
        Guid studyTemplateId,
        CreateInputParameterCommand command,
        [AsParameters] InputParameterServices services,
        CancellationToken ct)
    {
        var result = await services.Commands.CreateAsync(studyTemplateId, command, ct);

        if (result.IsFailure) return HandleFailure(result.Error!);

        var parameterId = result.GetValue();
        return Results.Created($"/api/studyTemplates/{studyTemplateId}/parameters/{parameterId}",
            new { id = parameterId });
    }

    private static async Task<IResult> Delete(
        Guid studyTemplateId,
        Guid parameterId,
        [AsParameters] InputParameterServices services,
        CancellationToken ct)
    {
        var result = await services.Commands.RemoveAsync(studyTemplateId, parameterId, ct);

        return result.IsFailure ? HandleFailure(result.Error!) : Results.NoContent();
    }

    private static async Task<IResult> Update(
        Guid studyTemplateId,
        Guid parameterId,
        UpdateInputParameterCommand command,
        [AsParameters] InputParameterServices services,
        CancellationToken ct)
    {
        var result = await services.Commands.UpdateAsync(
            studyTemplateId, parameterId, command, ct);
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
