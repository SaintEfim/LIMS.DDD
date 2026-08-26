using Carter;
using LIMS.Service.LaboratoryOperations.Application.StudyTemplates;
using Microsoft.AspNetCore.Mvc;

namespace LIMS.Service.LaboratoryOperations.API.Apis.StudyTemplateSnapshot;

public class StudyTemplateSnapshotModule
    : ModuleBase,
        ICarterModule
{
    public void AddRoutes(
        IEndpointRouteBuilder app)
    {
        var group = app.MapGroup("/api/study-template-snapshots")
            .WithTags("StudyTemplateSnapshots");

        group.MapGet("/", GetAll)
            .Produces<ICollection<StudyTemplateDto>>();

        group.MapGet("/{studyTemplateId:guid}", GetById)
            .Produces<StudyTemplateDto>()
            .Produces(StatusCodes.Status404NotFound);
    }

    private static async Task<IResult> GetAll(
        [FromServices] StudyTemplateSnapshotServices services,
        CancellationToken cancellationToken = default)
    {
        var templates = await services.Queries.GetAllAsync(cancellationToken);
        return Results.Ok(templates);
    }

    private static async Task<IResult> GetById(
        Guid studyTemplateId,
        [FromServices] StudyTemplateSnapshotServices services,
        CancellationToken cancellationToken = default)
    {
        var template = await services.Queries.GetByIdAsync(studyTemplateId, cancellationToken);
        return template is not null ? Results.Ok(template) : Results.NotFound();
    }
}
