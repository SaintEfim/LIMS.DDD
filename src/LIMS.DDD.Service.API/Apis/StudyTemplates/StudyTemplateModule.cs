using Carter;
using LIMS.DDD.Service.Application.StudyTemplates.Commands;
using Microsoft.AspNetCore.Mvc;

namespace LIMS.DDD.Service.API.Apis.StudyTemplates;

public class StudyTemplateModule : ICarterModule
{
    public void AddRoutes(
        IEndpointRouteBuilder app)
    {
        var group = app.MapGroup("/api/studyTemplates")
            .WithTags("StudyTemplates");

        group.MapGet("/", async (
            [FromServices] StudyTemplateServices services,
            CancellationToken ct = default) =>
        {
            var studyTemplates = await services.Queries.GetAllAsync(ct);
            return studyTemplates;
        });

        group.MapGet("/{id:guid}", async (
            Guid id,
            [FromServices] StudyTemplateServices services,
            CancellationToken ct = default) =>
        {
            var dto = await services.Queries.GetByIdAsync(id, ct);
            return dto is not null ? Results.Ok(dto) : Results.NotFound();
        });

        group.MapPost("/", async (
            CreateStudyTemplateCommand createCommand,
            [FromServices] StudyTemplateServices services,
            CancellationToken ct = default) =>
        {
            var studyTemplateId = await services.Commands.CreateAsync(createCommand, ct);

            return Results.Created($"/api/studyTemplates", new { id = studyTemplateId });
        });

        group.MapPatch("/{id:guid}", async (
            Guid id,
            UpdateStudyTemplateCommand updateCommand,
            [FromServices] StudyTemplateServices services,
            CancellationToken ct = default) =>
        {
            var isUpdated = await services.Commands.UpdateAsync(id, updateCommand, ct);

            return isUpdated ? Results.NoContent() : Results.NotFound();
        });

        group.MapDelete("/{id:guid}", async (
            Guid id,
            [FromServices] StudyTemplateServices services,
            CancellationToken ct = default) =>
        {
            var isDeleted = await services.Commands.DeleteAsync(id, ct);

            return isDeleted ? Results.NoContent() : Results.NotFound();
        });

        group.MapPost("/{id:guid}/status", async (
            Guid id,
            ChangeStatusCommand command,
            [FromServices] StudyTemplateCommands commands,
            CancellationToken ct) =>
        {
            var isChanged = await commands.ChangeStatusAsync(id, command, ct);

            return isChanged ? Results.NoContent() : Results.NotFound();
        });
    }
}
