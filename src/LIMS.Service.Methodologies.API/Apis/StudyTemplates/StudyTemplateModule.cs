using Carter;
using LIMS.Service.Methodologies.Application.StudyTemplates.Core;
using LIMS.Service.Methodologies.Application.StudyTemplates.Core.Commands;
using Microsoft.AspNetCore.Mvc;

namespace LIMS.Service.Methodologies.API.Apis.StudyTemplates;

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

        group.MapDelete("/{id:guid}", Delete)
            .Produces(StatusCodes.Status204NoContent)
            .Produces(StatusCodes.Status404NotFound)
            .Produces(StatusCodes.Status400BadRequest)
            .Produces(StatusCodes.Status500InternalServerError);
    }

    private static async Task<IResult> GetAl(
        [FromServices] StudyTemplateServices services,
        CancellationToken cancellationToken = default)
    {
        var studyTemplates = await services.Queries.GetAllAsync(cancellationToken);
        return Results.Ok(studyTemplates);
    }

    private static async Task<IResult> GetById(
        Guid id,
        [FromServices] StudyTemplateServices services,
        CancellationToken cancellationToken = default)
    {
        var dto = await services.Queries.GetByIdAsync(id, cancellationToken);
        return dto is null ? Results.NotFound() : Results.Ok(dto);
    }

    private static async Task<IResult> Create(
        CreateStudyTemplateCommand createCommand,
        [FromServices] StudyTemplateServices services,
        CancellationToken cancellationToken = default)
    {
        var result = await services.Commands.CreateAsync(createCommand, cancellationToken);

        if (result.IsFailure)
        {
            return HandleFailure(result.GetError());
        }

        var createdId = result.GetValue()
            .Id.Value;
        return Results.Created($"/api/studyTemplates/{createdId}", new { id = createdId });
    }

    private static async Task<IResult> Update(
        Guid id,
        UpdateStudyTemplateCommand updateCommand,
        [FromServices] StudyTemplateServices services,
        CancellationToken cancellationToken = default)
    {
        var result = await services.Commands.UpdateAsync(id, updateCommand, cancellationToken);
        return result.IsFailure ? HandleFailure(result.GetError()) : Results.NoContent();
    }

    private static async Task<IResult> ChangeStatus(
        Guid id,
        string newStatus,
        [FromServices] StudyTemplateServices services,
        CancellationToken cancellationToken = default)
    {
        var result = await services.Commands.ChangeStatusAsync(id, newStatus, cancellationToken);
        return result.IsFailure ? HandleFailure(result.GetError()) : Results.Ok();
    }

    private static async Task<IResult> CreateRevision(
        Guid id,
        CreateStudyTemplateRevisionCommand command,
        [FromServices] StudyTemplateServices services,
        CancellationToken cancellationToken = default)
    {
        var result = await services.Commands.CreateRevisionAsync(id, command, cancellationToken);

        if (result.IsFailure)
        {
            return HandleFailure(result.GetError());
        }

        var newTemplateId = result.GetValue();
        return Results.Created($"/api/study-templates/{newTemplateId}", new { id = newTemplateId });
    }

    private static async Task<IResult> Delete(
        Guid id,
        [FromServices] StudyTemplateServices services,
        CancellationToken cancellationToken = default)
    {
        var result = await services.Commands.DeleteAsync(id, cancellationToken);

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
