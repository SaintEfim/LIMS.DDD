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

        group.MapGet("/", GetAllInputParameters)
            .Produces<ICollection<InputParameterDto>>();

        group.MapGet("/{parameterId:guid}", GetInputParameterById)
            .Produces<InputParameterDto>()
            .Produces(StatusCodes.Status404NotFound);

        group.MapPost("/", CreateInputParameter)
            .Produces(StatusCodes.Status201Created)
            .Produces(StatusCodes.Status404NotFound)
            .Produces(StatusCodes.Status400BadRequest)
            .Produces(StatusCodes.Status500InternalServerError);

        group.MapDelete("/{parameterId:guid}", DeleteInputParameter)
            .Produces(StatusCodes.Status204NoContent)
            .Produces(StatusCodes.Status404NotFound)
            .Produces(StatusCodes.Status400BadRequest)
            .Produces(StatusCodes.Status500InternalServerError);
    }

    private static async Task<IResult> GetAllInputParameters(
        Guid studyTemplateId,
        [AsParameters] InputParameterServices services,
        CancellationToken ct)
    {
        var parameters = await services.Queries.GetAllByTemplateIdAsync(studyTemplateId, ct);
        return Results.Ok(parameters);
    }

    private static async Task<IResult> GetInputParameterById(
        Guid studyTemplateId,
        Guid parameterId,
        [AsParameters] InputParameterServices services,
        CancellationToken ct)
    {
        var parameter = await services.Queries.GetByIdAsync(studyTemplateId, parameterId, ct);
        return parameter is not null ? Results.Ok(parameter) : Results.NotFound();
    }

    private static async Task<IResult> CreateInputParameter(
        Guid studyTemplateId,
        CreateInputParameterCommand command,
        [AsParameters] InputParameterServices services,
        CancellationToken ct)
    {
        var result = await services.Commands.AddInputParameterAsync(studyTemplateId, command, ct);

        if (result.IsFailure) return HandleFailure(result.Error!);

        var parameterId = result.Value;
        return Results.Created($"/api/studyTemplates/{studyTemplateId}/parameters/{parameterId}",
            new { id = parameterId });
    }

    private static async Task<IResult> DeleteInputParameter(
        Guid studyTemplateId,
        Guid parameterId,
        [AsParameters] InputParameterServices services,
        CancellationToken ct)
    {
        var result = await services.Commands.RemoveInputParameterAsync(studyTemplateId, parameterId, ct);

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
