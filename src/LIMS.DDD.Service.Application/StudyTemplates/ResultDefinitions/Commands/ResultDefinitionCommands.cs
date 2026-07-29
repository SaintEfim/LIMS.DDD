using LIMS.DDD.Service.Domain.SeedWork.Result;
using LIMS.DDD.Service.Domain.StudyTemplateAggregate;
using LIMS.DDD.Service.Domain.StudyTemplateAggregate.Ids;
using LIMS.DDD.Service.Domain.StudyTemplateAggregate.ValueObjects;

namespace LIMS.DDD.Service.Application.StudyTemplates.ResultDefinitions.Commands;

public sealed class ResultDefinitionCommands(IStudyTemplateRepository repository)
{
    public async Task<Result<Guid, Exception>> AddResultDefinitionAsync(
        Guid studyTemplateId,
        CreateResultDefinitionCommand command,
        CancellationToken cancellationToken = default)
    {
        var templateResult = await GetTemplateForChangeAsync(studyTemplateId, cancellationToken);
        if (templateResult.IsFailure) return Result<Guid, Exception>.Failure(templateResult.Error!);

        var specification = new Specification(command.MinValue, command.MaxValue);

        var addResult = templateResult.Value!.AddResultDefinition(command.ResultInstance, command.Unit, specification);
        if (addResult.IsFailure) return Result<Guid, Exception>.Failure(addResult.Error!);

        var saveResult = await SaveChangesAsync(cancellationToken);
        return saveResult.IsFailure
            ? Result<Guid, Exception>.Failure(saveResult.Error!)
            : Result<Guid, Exception>.Success(addResult.Value!.Id.Value);
    }

    public async Task<Result<Exception>> RemoveResultDefinitionAsync(
        Guid studyTemplateId,
        Guid resultId,
        CancellationToken cancellationToken = default)
    {
        var templateResult = await GetTemplateForChangeAsync(studyTemplateId, cancellationToken);
        if (templateResult.IsFailure) return Result<Exception>.Failure(templateResult.Error!);

        var removeResult = templateResult.Value!.RemoveResultDefinition(new ResultDefinitionId(resultId));
        if (removeResult.IsFailure) return Result<Exception>.Failure(removeResult.Error!);

        return await SaveChangesAsync(cancellationToken);
    }

    public async Task<Result<Exception>> UpdateResultDefinitionAsync(
        Guid studyTemplateId,
        Guid resultDefinitionId,
        UpdateResultDefinitionCommand command,
        CancellationToken cancellationToken = default)
    {
        var templateResult = await GetTemplateForChangeAsync(studyTemplateId, cancellationToken);
        if (templateResult.IsFailure) return Result<Exception>.Failure(templateResult.Error!);

        Specification? specification = null;
        if (command.MinValue is not null || command.MaxValue is not null)
            specification = new Specification(command.MinValue, command.MaxValue);

        var updateResult = templateResult.Value!.UpdateResultDefinition(new ResultDefinitionId(resultDefinitionId),
            command.ResultInstance, command.Unit, specification);
        if (updateResult.IsFailure) return Result<Exception>.Failure(updateResult.Error!);

        return await SaveChangesAsync(cancellationToken);
    }

    private async Task<Result<StudyTemplate, Exception>> GetTemplateForChangeAsync(
        Guid studyTemplateId,
        CancellationToken cancellationToken)
    {
        var template = await repository.GetByIdForChangeAsync(new StudyTemplateId(studyTemplateId), cancellationToken);
        return template is null
            ? Result<StudyTemplate, Exception>.Failure(
                new KeyNotFoundException($"StudyTemplate with id {studyTemplateId} not found."))
            : Result<StudyTemplate, Exception>.Success(template);
    }

    private async Task<Result<Exception>> SaveChangesAsync(
        CancellationToken cancellationToken)
    {
        try
        {
            await repository.SaveChangesAsync(cancellationToken);
            return Result<Exception>.Success();
        }
        catch (Exception ex)
        {
            return Result<Exception>.Failure(new Exception($"Failed to save changes: {ex.Message}", ex));
        }
    }
}
