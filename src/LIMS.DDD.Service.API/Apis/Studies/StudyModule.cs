using Carter;
using LIMS.DDD.Service.Application.Studies.Core;
using LIMS.DDD.Service.Application.Studies.Core.Commands;
using LIMS.DDD.Service.Domain.LaboratoryOperationsContext.SampleAggregate;

namespace LIMS.DDD.Service.API.Apis.Studies;

public class StudyModule : ICarterModule
{
    public void AddRoutes(
        IEndpointRouteBuilder app)
    {
        var group = app.MapGroup("/api/samples/{sampleId:guid}/studies")
            .WithTags("Studies");

        group.MapGet("/", GetAll)
            .Produces<ICollection<StudyDto>>();

        group.MapGet("/{studyId:guid}", GetById)
            .Produces<StudyDto>()
            .Produces(StatusCodes.Status404NotFound);

        group.MapPost("/", Create)
            .Produces(StatusCodes.Status201Created)
            .Produces(StatusCodes.Status404NotFound)
            .Produces(StatusCodes.Status400BadRequest)
            .Produces(StatusCodes.Status500InternalServerError);

        group.MapPatch("/{studyId:guid}/notes", UpdateNotes)
            .Produces(StatusCodes.Status204NoContent)
            .Produces(StatusCodes.Status404NotFound)
            .Produces(StatusCodes.Status400BadRequest);

        group.MapPost("/{studyId:guid}/reassign-sample", ReassignSample)
            .Produces(StatusCodes.Status204NoContent)
            .Produces(StatusCodes.Status404NotFound)
            .Produces(StatusCodes.Status400BadRequest);

        group.MapPost("/{studyId:guid}/change-status", ChangeStatus)
            .Produces(StatusCodes.Status204NoContent)
            .Produces(StatusCodes.Status404NotFound)
            .Produces(StatusCodes.Status400BadRequest);

        group.MapDelete("/{studyId:guid}", Delete)
            .Produces(StatusCodes.Status204NoContent)
            .Produces(StatusCodes.Status404NotFound)
            .Produces(StatusCodes.Status400BadRequest);
    }

    private static async Task<IResult> GetAll(
        Guid sampleId,
        [AsParameters] StudyServices services,
        CancellationToken cancellationToken = default)
    {
        var studies = await services.Queries.GetAllBySampleIdAsync(sampleId, cancellationToken);
        return Results.Ok(studies);
    }

    private static async Task<IResult> GetById(
        Guid sampleId,
        Guid studyId,
        [AsParameters] StudyServices services,
        CancellationToken cancellationToken = default)
    {
        var dto = await services.Queries.GetByIdAsync(studyId, cancellationToken);
        return dto is not null ? Results.Ok(dto) : Results.NotFound();
    }

    private static async Task<IResult> Create(
        Guid sampleId,
        CreateStudyCommand command,
        [AsParameters] StudyServices services,
        CancellationToken cancellationToken = default)
    {
        var result = await services.Commands.CreateAsync(new SampleId(sampleId), command, cancellationToken);

        if (result.IsFailure)
        {
            return HandleFailure(result.GetError());
        }

        var createdId = result.GetValue()
            .Id.Value;
        return Results.Created($"/api/samples/{sampleId}/studies/{createdId}", new { id = createdId });
    }

    private static async Task<IResult> UpdateNotes(
        Guid sampleId,
        Guid studyId,
        UpdateStudyNotesCommand command,
        [AsParameters] StudyServices services,
        CancellationToken cancellationToken = default)
    {
        var result = await services.Commands.UpdateNotesAsync(studyId, command, cancellationToken);
        return result.IsFailure ? HandleFailure(result.GetError()) : Results.NoContent();
    }

    private static async Task<IResult> ReassignSample(
        Guid sampleId,
        Guid studyId,
        ReassignStudySampleCommand command,
        [AsParameters] StudyServices services,
        CancellationToken cancellationToken = default)
    {
        var result = await services.Commands.ReassignSampleAsync(studyId, command, cancellationToken);
        return result.IsFailure ? HandleFailure(result.GetError()) : Results.NoContent();
    }

    private static async Task<IResult> ChangeStatus(
        Guid sampleId,
        Guid studyId,
        ChangeStudyStatusCommand command,
        [AsParameters] StudyServices services,
        CancellationToken cancellationToken = default)
    {
        var result = await services.Commands.ChangeStatusAsync(studyId, command, cancellationToken);
        return result.IsFailure ? HandleFailure(result.GetError()) : Results.NoContent();
    }

    private static async Task<IResult> Delete(
        Guid sampleId,
        Guid studyId,
        [AsParameters] StudyServices services,
        CancellationToken cancellationToken = default)
    {
        var result = await services.Commands.DeleteAsync(studyId, cancellationToken);
        return result.IsFailure ? HandleFailure(result.GetError()) : Results.NoContent();
    }

    private static IResult HandleFailure(
        Exception error)
    {
        return error switch
        {
            KeyNotFoundException => Results.NotFound(new { error.Message }),
            ArgumentException or InvalidOperationException => Results.BadRequest(new { error.Message }),
            _ => Results.Problem(error.Message, statusCode: StatusCodes.Status500InternalServerError,
                title: "An unexpected error occurred")
        };
    }
}
