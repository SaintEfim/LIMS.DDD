using Carter;
using LIMS.DDD.Service.Application.StudyTemplates.Commands;
using LIMS.DDD.Service.Application.StudyTemplates.Queries;

namespace LIMS.DDD.Service.API.Apis.StudyTemplates;

public class StudyTemplateModule : ICarterModule
{
    public void AddRoutes(
        IEndpointRouteBuilder app)
    {
        var group = app.MapGroup("/api/study-templates")
            .WithTags("StudyTemplates");

        group.MapGet("/", GetAl)
            .Produces<ICollection<StudyTemplateDto>>();

        group.MapGet("/{id:guid}", GetById)
            .Produces<StudyTemplateDto>()
            .Produces(StatusCodes.Status404NotFound);

        group.MapPost("/", Create)
            .Produces(StatusCodes.Status201Created)
            .Produces(StatusCodes.Status400BadRequest)
            .Produces(StatusCodes.Status500InternalServerError);

        group.MapPatch("/{id:guid}", Update)
            .Produces(StatusCodes.Status204NoContent)
            .Produces(StatusCodes.Status404NotFound)
            .Produces(StatusCodes.Status400BadRequest);

        group.MapPost("/{id:guid}/change-status", ChangeStatus)
            .WithName("ApproveStudyTemplate")
            .Produces(StatusCodes.Status200OK)
            .Produces(StatusCodes.Status404NotFound)
            .Produces(StatusCodes.Status400BadRequest);

        group.MapPost("/{id:guid}/create-revision", CreateRevision)
            .WithName("CreateStudyTemplateRevision")
            .Produces(StatusCodes.Status201Created)
            .Produces(StatusCodes.Status404NotFound)
            .Produces(StatusCodes.Status400BadRequest)
            .Produces(StatusCodes.Status500InternalServerError);
    }

    private static async Task<IResult> GetAl(
        [AsParameters] StudyTemplateServices services,
        CancellationToken ct)
    {
        var studyTemplates = await services.Queries.GetAllAsync(ct);
        return Results.Ok(studyTemplates);
    }

    private static async Task<IResult> GetById(
        Guid id,
        [AsParameters] StudyTemplateServices services,
        CancellationToken ct)
    {
        var dto = await services.Queries.GetByIdAsync(id, ct);
        return dto is null ? Results.NotFound() : Results.Ok(dto);
    }

    private static async Task<IResult> Create(
        CreateStudyTemplateCommand createCommand,
        [AsParameters] StudyTemplateServices services,
        CancellationToken ct)
    {
        var result = await services.Commands.CreateAsync(createCommand, ct);

        if (result.IsFailure) return HandleFailure(result.Error!);

        var createdId = result.GetValue().Id.Value;
        return Results.Created($"/api/studyTemplates/{createdId}", new { id = createdId });
    }

    private static async Task<IResult> Update(
        Guid id,
        UpdateStudyTemplateCommand updateCommand,
        [AsParameters] StudyTemplateServices services,
        CancellationToken ct)
    {
        var result = await services.Commands.UpdateAsync(id, updateCommand, ct);
        return result.IsFailure ? HandleFailure(result.Error!) : Results.NoContent();
    }

    private static async Task<IResult> ChangeStatus(
        Guid id,
        string newStatus,
        [AsParameters] StudyTemplateServices services,
        CancellationToken ct)
    {
        var result = await services.Commands.ChangeStatusAsync(id, newStatus, ct);
        return result.IsFailure ? HandleFailure(result.Error!) : Results.Ok();
    }

    private static async Task<IResult> CreateRevision(
        Guid id,
        CreateStudyTemplateRevisionCommand command,
        [AsParameters] StudyTemplateServices services,
        CancellationToken ct)
    {
        var result = await services.Commands.CreateRevisionAsync(id, command, ct);

        if (result.IsFailure) return HandleFailure(result.Error!);

        var newTemplateId = result.GetValue();
        return Results.Created($"/api/study-templates/{newTemplateId}", new { id = newTemplateId });
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
