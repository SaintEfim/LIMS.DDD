using Carter;
using LIMS.DDD.Service.Application.StudyTemplates.StudyTemplateObservations.Commands;
using LIMS.DDD.Service.Application.StudyTemplates.StudyTemplateObservations.Queries;

namespace LIMS.DDD.Service.API.Apis.StudyTemplateObservations;

public class StudyTemplateObservationModule : ICarterModule
{
    public void AddRoutes(
        IEndpointRouteBuilder app)
    {
        var group = app.MapGroup("/api/studyTemplates/{studyTemplateId:guid}/observations")
            .WithTags("StudyTemplateObservations");

        group.MapGet("/", GetAllObservations)
            .Produces<ICollection<StudyTemplateObservationDto>>();

        group.MapGet("/{parameterId:guid}", GetObservationById)
            .Produces<StudyTemplateObservationDto>()
            .Produces(StatusCodes.Status404NotFound);

        group.MapPost("/", CreateObservation)
            .Produces(StatusCodes.Status201Created)
            .Produces(StatusCodes.Status404NotFound)
            .Produces(StatusCodes.Status400BadRequest)
            .Produces(StatusCodes.Status500InternalServerError);

        group.MapDelete("/{parameterId:guid}", DeleteObservation)
            .Produces(StatusCodes.Status204NoContent)
            .Produces(StatusCodes.Status404NotFound)
            .Produces(StatusCodes.Status400BadRequest)
            .Produces(StatusCodes.Status500InternalServerError);
    }

    private static async Task<IResult> GetAllObservations(
        Guid studyTemplateId,
        [AsParameters] StudyTemplateObservationServices services,
        CancellationToken ct)
    {
        var parameters = await services.Queries.GetAllByTemplateIdAsync(studyTemplateId, ct);
        return Results.Ok(parameters);
    }

    private static async Task<IResult> GetObservationById(
        Guid studyTemplateId,
        Guid parameterId,
        [AsParameters] StudyTemplateObservationServices services,
        CancellationToken ct)
    {
        var parameter = await services.Queries.GetByIdAsync(studyTemplateId, parameterId, ct);
        return parameter is not null ? Results.Ok(parameter) : Results.NotFound();
    }

    private static async Task<IResult> CreateObservation(
        Guid studyTemplateId,
        CreateStudyTemplateObservationCommand command,
        [AsParameters] StudyTemplateObservationServices services,
        CancellationToken ct)
    {
        var result = await services.Commands.AddAddStudyTemplateObservationAsync(studyTemplateId, command, ct);

        if (result.IsFailure) return HandleFailure(result.Error!);

        var parameterId = result.Value;
        return Results.Created($"/api/studyTemplates/{studyTemplateId}/parameters/{parameterId}",
            new { id = parameterId });
    }

    private static async Task<IResult> DeleteObservation(
        Guid studyTemplateId,
        Guid parameterId,
        [AsParameters] StudyTemplateObservationServices services,
        CancellationToken ct)
    {
        var result = await services.Commands.RemoveRemoveStudyTemplateObservationAsync(studyTemplateId, parameterId, ct);

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
