using Carter;
using LIMS.DDD.Service.Application.StudyTemplates.StudyTemplateParameters.Commands;
using Microsoft.AspNetCore.Mvc;

namespace LIMS.DDD.Service.API.Apis.StudyTemplateParameters;

public class StudyTemplateParameterModule : ICarterModule
{
    public void AddRoutes(
        IEndpointRouteBuilder app)
    {
        var group = app.MapGroup("/api/studyTemplates/{studyTemplateId:guid}")
            .WithTags("StudyTemplateParameters");

        group.MapGet("/parameters", async (
            Guid studyTemplateId,
            [FromServices] StudyTemplateParameterServices services,
            CancellationToken ct = default) =>
        {
            var parameters = await services.Queries.GetAllByTemplateIdAsync(studyTemplateId, ct);
            return parameters;
        });

        group.MapGet("/parameters/{parameterId:guid}", async (
            Guid studyTemplateId,
            Guid parameterId,
            [FromServices] StudyTemplateParameterServices services,
            CancellationToken ct = default) =>
        {
            var parameter = await services.Queries.GetByIdAsync(studyTemplateId, parameterId, ct);
            return parameter is not null ? Results.Ok((object?) parameter) : Results.NotFound();
        });

        group.MapPost("/parameters", async (
            Guid studyTemplateId,
            CreateStudyTemplateParameterCommand command,
            [FromServices] StudyTemplateParameterServices services,
            CancellationToken ct = default) =>
        {
            var parameterId = await services.Commands.AddStudyTemplateParameterAsync(studyTemplateId, command, ct);

            return Results.Created($"/api/studyTemplates/{studyTemplateId}/parameters/{parameterId}",
                new { id = parameterId });
        });

        group.MapDelete("/parameters/{parameterId:guid}", async (
            Guid studyTemplateId,
            Guid parameterId,
            [FromServices] StudyTemplateParameterServices services,
            CancellationToken ct = default) =>
        {
            await services.Commands.RemoveStudyTemplateParameterAsync(studyTemplateId, parameterId, ct);
            return Results.NoContent();
        });
    }
}
