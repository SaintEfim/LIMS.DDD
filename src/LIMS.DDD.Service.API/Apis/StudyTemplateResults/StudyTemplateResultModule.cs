using Carter;
using LIMS.DDD.Service.Application.StudyTemplates.StudyTemplateResults.Commands;
using Microsoft.AspNetCore.Mvc;

namespace LIMS.DDD.Service.API.Apis.StudyTemplateResults;

public class StudyTemplateResultApi : ICarterModule
{
    public void AddRoutes(
        IEndpointRouteBuilder app)
    {
        var group = app.MapGroup("/api/studyTemplates/{studyTemplateId:guid}")
            .WithTags("StudyTemplateResults");

        group.MapGet("/results", async (
            Guid studyTemplateId,
            [FromServices] StudyTemplateResultServices services,
            CancellationToken ct = default) =>
        {
            var results = await services.Queries.GetAllByTemplateIdAsync(studyTemplateId, ct);
            return results;
        });

        group.MapGet("/results/{resultId:guid}", async (
            Guid studyTemplateId,
            Guid resultId,
            [FromServices] StudyTemplateResultServices services,
            CancellationToken ct = default) =>
        {
            var result = await services.Queries.GetByIdAsync(studyTemplateId, resultId, ct);
            return result is not null ? Results.Ok((object?) result) : Results.NotFound();
        });

        group.MapPost("/results", async (
            Guid studyTemplateId,
            CreateStudyTemplateResultCommand command,
            [FromServices] StudyTemplateResultServices services,
            CancellationToken ct = default) =>
        {
            var resultId = await services.Commands.AddStudyTemplateResultAsync(studyTemplateId, command, ct);

            return Results.Created($"/api/studyTemplates/{studyTemplateId}/results/{resultId}", new { id = resultId });
        });

        group.MapDelete("/results/{resultId:guid}", async (
            Guid studyTemplateId,
            Guid resultId,
            [FromServices] StudyTemplateResultServices services,
            CancellationToken ct = default) =>
        {
            var isRemoved = await services.Commands.RemoveStudyTemplateResultAsync(studyTemplateId, resultId, ct);
            return isRemoved ? Results.NoContent() : Results.NotFound();
        });
    }
}
