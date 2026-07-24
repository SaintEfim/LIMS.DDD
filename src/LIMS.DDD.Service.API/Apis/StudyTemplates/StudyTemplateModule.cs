using Carter;
using LIMS.DDD.Service.Application.StudyTemplates.Commands;
using LIMS.DDD.Service.Application.StudyTemplates.Queries;

namespace LIMS.DDD.Service.API.Apis.StudyTemplates;

public class StudyTemplateModule : ICarterModule
{
    public void AddRoutes(
        IEndpointRouteBuilder app)
    {
        var group = app.MapGroup("/api/studyTemplates")
            .WithTags("StudyTemplates");

        group.MapGet("/", GetAllStudyTemplates)
            .Produces<ICollection<StudyTemplateDto>>();

        group.MapGet("/{id:guid}", GetStudyTemplateById)
            .Produces<StudyTemplateDto>()
            .Produces(StatusCodes.Status404NotFound);

        group.MapPost("/", CreateStudyTemplate)
            .Produces(StatusCodes.Status201Created)
            .Produces(StatusCodes.Status400BadRequest)
            .Produces(StatusCodes.Status500InternalServerError);

        group.MapPatch("/{id:guid}", UpdateStudyTemplate)
            .Produces(StatusCodes.Status204NoContent)
            .Produces(StatusCodes.Status404NotFound)
            .Produces(StatusCodes.Status400BadRequest);

        group.MapDelete("/{id:guid}", DeleteStudyTemplate)
            .Produces(StatusCodes.Status204NoContent)
            .Produces(StatusCodes.Status404NotFound);

        group.MapPost("/{id:guid}/status", ChangeStudyTemplateStatus)
            .Produces(StatusCodes.Status204NoContent)
            .Produces(StatusCodes.Status404NotFound)
            .Produces(StatusCodes.Status400BadRequest);
    }

    private static async Task<IResult> GetAllStudyTemplates(
        [AsParameters] StudyTemplateServices services,
        CancellationToken ct)
    {
        var studyTemplates = await services.Queries.GetAllAsync(ct);
        return Results.Ok(studyTemplates);
    }

    private static async Task<IResult> GetStudyTemplateById(
        Guid id,
        [AsParameters] StudyTemplateServices services,
        CancellationToken ct)
    {
        var dto = await services.Queries.GetByIdAsync(id, ct);
        return dto is null ? Results.NotFound() : Results.Ok(dto);
    }

    private static async Task<IResult> CreateStudyTemplate(
        CreateStudyTemplateCommand createCommand,
        [AsParameters] StudyTemplateServices services,
        CancellationToken ct)
    {
        var result = await services.Commands.CreateAsync(createCommand, ct);

        if (result.IsFailure) return HandleFailure(result.Error!);

        var createdId = result.Value!.Id.Value;
        return Results.Created($"/api/studyTemplates/{createdId}", new { id = createdId });
    }

    private static async Task<IResult> UpdateStudyTemplate(
        Guid id,
        UpdateStudyTemplateCommand updateCommand,
        [AsParameters] StudyTemplateServices services,
        CancellationToken ct)
    {
        var result = await services.Commands.UpdateAsync(id, updateCommand, ct);
        return result.IsFailure ? HandleFailure(result.Error!) : Results.NoContent();
    }

    private static async Task<IResult> DeleteStudyTemplate(
        Guid id,
        [AsParameters] StudyTemplateServices services,
        CancellationToken ct)
    {
        var result = await services.Commands.DeleteAsync(id, ct);
        return result.IsFailure ? HandleFailure(result.Error!) : Results.NoContent();
    }

    private static async Task<IResult> ChangeStudyTemplateStatus(
        Guid id,
        ChangeStatusCommand command,
        [AsParameters] StudyTemplateServices services,
        CancellationToken ct)
    {
        var result = await services.Commands.ChangeStatusAsync(id, command, ct);
        return result.IsFailure ? HandleFailure(result.Error!) : Results.NoContent();
    }

    private static IResult HandleFailure(
        Exception error) =>
        error switch
        {
            KeyNotFoundException => Results.NotFound(new { error.Message }),
            ArgumentException or InvalidOperationException => Results.BadRequest(new { error.Message }),
            _ => Results.Problem(detail: error.Message, statusCode: StatusCodes.Status500InternalServerError,
                title: "An unexpected error occurred")
        };
}
