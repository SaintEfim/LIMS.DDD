using Carter;
using LIMS.DDD.Service.Application.StudyTemplates.StudyTemplateParameters.Commands;
using LIMS.DDD.Service.Application.StudyTemplates.StudyTemplateParameters.Queries;

namespace LIMS.DDD.Service.API.Apis.StudyTemplateParameters;

public class StudyTemplateParameterModule : ICarterModule
{
    public void AddRoutes(
        IEndpointRouteBuilder app)
    {
        var group = app.MapGroup("/api/studyTemplates/{studyTemplateId:guid}/parameters")
            .WithTags("StudyTemplateParameters");

        group.MapGet("/", GetAllParameters)
            .Produces<ICollection<StudyTemplateParameterDto>>();

        group.MapGet("/{parameterId:guid}", GetParameterById)
            .Produces<StudyTemplateParameterDto>()
            .Produces(StatusCodes.Status404NotFound);

        group.MapPost("/", CreateParameter)
            .Produces(StatusCodes.Status201Created)
            .Produces(StatusCodes.Status404NotFound)
            .Produces(StatusCodes.Status400BadRequest)
            .Produces(StatusCodes.Status500InternalServerError);

        group.MapDelete("/{parameterId:guid}", DeleteParameter)
            .Produces(StatusCodes.Status204NoContent)
            .Produces(StatusCodes.Status404NotFound)
            .Produces(StatusCodes.Status400BadRequest)
            .Produces(StatusCodes.Status500InternalServerError);
    }

    private static async Task<IResult> GetAllParameters(
        Guid studyTemplateId,
        [AsParameters] StudyTemplateParameterServices services,
        CancellationToken ct)
    {
        var parameters = await services.Queries.GetAllByTemplateIdAsync(studyTemplateId, ct);
        return Results.Ok(parameters);
    }

    private static async Task<IResult> GetParameterById(
        Guid studyTemplateId,
        Guid parameterId,
        [AsParameters] StudyTemplateParameterServices services,
        CancellationToken ct)
    {
        var parameter = await services.Queries.GetByIdAsync(studyTemplateId, parameterId, ct);
        return parameter is not null ? Results.Ok(parameter) : Results.NotFound();
    }

    private static async Task<IResult> CreateParameter(
        Guid studyTemplateId,
        CreateStudyTemplateParameterCommand command,
        [AsParameters] StudyTemplateParameterServices services,
        CancellationToken ct)
    {
        var result = await services.Commands.AddStudyTemplateParameterAsync(studyTemplateId, command, ct);

        if (result.IsFailure) return HandleFailure(result.Error!);

        var parameterId = result.Value;
        return Results.Created($"/api/studyTemplates/{studyTemplateId}/parameters/{parameterId}",
            new { id = parameterId });
    }

    private static async Task<IResult> DeleteParameter(
        Guid studyTemplateId,
        Guid parameterId,
        [AsParameters] StudyTemplateParameterServices services,
        CancellationToken ct)
    {
        var result = await services.Commands.RemoveStudyTemplateParameterAsync(studyTemplateId, parameterId, ct);

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
