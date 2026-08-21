using Carter;
using LIMS.Service.Methodologies.Application.StudyTemplates.InputParameters;
using LIMS.Service.Methodologies.Application.StudyTemplates.InputParameters.Commands;
using Microsoft.AspNetCore.Mvc;

namespace LIMS.Service.Methodologies.API.Apis.InputParameters;

public class InputParameterModule
    : ModuleBase,
        ICarterModule
{
    public void AddRoutes(
        IEndpointRouteBuilder app)
    {
        var group = app.MapGroup("/api/study-templates/{studyTemplateId:guid}/input-parameters")
            .WithTags("InputParameters");

        group.MapGet("/", GetAll)
            .Produces<ICollection<InputParameterDto>>();

        group.MapGet("/{parameterId:guid}", GetById)
            .Produces<InputParameterDto>()
            .Produces(StatusCodes.Status404NotFound);

        group.MapPost("/", Create)
            .Produces(StatusCodes.Status201Created)
            .Produces(StatusCodes.Status404NotFound)
            .Produces(StatusCodes.Status400BadRequest)
            .Produces(StatusCodes.Status409Conflict)
            .Produces(StatusCodes.Status500InternalServerError);

        group.MapDelete("/{parameterId:guid}", Delete)
            .Produces(StatusCodes.Status204NoContent)
            .Produces(StatusCodes.Status404NotFound)
            .Produces(StatusCodes.Status409Conflict)
            .Produces(StatusCodes.Status500InternalServerError);

        group.MapPatch("/{parameterId:guid}", Update)
            .Produces(StatusCodes.Status204NoContent)
            .Produces(StatusCodes.Status404NotFound)
            .Produces(StatusCodes.Status400BadRequest)
            .Produces(StatusCodes.Status409Conflict);
    }

    private static async Task<IResult> GetAll(
        Guid studyTemplateId,
        [FromServices] InputParameterServices services,
        CancellationToken cancellationToken = default)
    {
        var parameters = await services.Queries.GetAllByTemplateIdAsync(studyTemplateId, cancellationToken);
        return Results.Ok(parameters);
    }

    private static async Task<IResult> GetById(
        Guid studyTemplateId,
        Guid parameterId,
        [FromServices] InputParameterServices services,
        CancellationToken cancellationToken = default)
    {
        var parameter = await services.Queries.GetByIdAsync(studyTemplateId, parameterId, cancellationToken);
        return parameter is not null ? Results.Ok(parameter) : Results.NotFound();
    }

    private static async Task<IResult> Create(
        Guid studyTemplateId,
        CreateInputParameterCommand command,
        [FromServices] InputParameterServices services,
        CancellationToken cancellationToken = default)
    {
        var result = await services.Commands.CreateAsync(studyTemplateId, command, cancellationToken);
        if (result.IsFailure)
        {
            return HandleFailure(result.GetError());
        }

        var parameterId = result.GetValue();
        return Results.Created($"/api/study-templates/{studyTemplateId}/input-parameters/{parameterId}",
            new { id = parameterId });
    }

    private static async Task<IResult> Delete(
        Guid studyTemplateId,
        Guid parameterId,
        [FromServices] InputParameterServices services,
        CancellationToken cancellationToken = default)
    {
        var result = await services.Commands.RemoveAsync(studyTemplateId, parameterId, cancellationToken);
        return result.IsFailure ? HandleFailure(result.GetError()) : Results.NoContent();
    }

    private static async Task<IResult> Update(
        Guid studyTemplateId,
        Guid parameterId,
        UpdateInputParameterCommand command,
        [FromServices] InputParameterServices services,
        CancellationToken cancellationToken = default)
    {
        var result = await services.Commands.UpdateAsync(studyTemplateId, parameterId, command, cancellationToken);
        return result.IsFailure ? HandleFailure(result.GetError()) : Results.NoContent();
    }
}
