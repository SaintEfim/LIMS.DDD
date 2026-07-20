using Carter;
using LIMS.DDD.Service.API.Apis;
using LIMS.DDD.Service.API.Dtos;
using Microsoft.AspNetCore.Mvc;

namespace LIMS.DDD.Service.API.Modules;

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
            return studyTemplates.Select(StudyTemplateDto.FromDomain)
                .ToList();
        });

        group.MapGet("/{id:guid}", async (
            Guid id,
            [FromServices] StudyTemplateServices services,
            CancellationToken ct = default) =>
        {
            var studyTemplate = await services.Queries.GetByIdAsync(id, ct);
            return StudyTemplateDto.FromDomain(studyTemplate);
        });
    }
}
