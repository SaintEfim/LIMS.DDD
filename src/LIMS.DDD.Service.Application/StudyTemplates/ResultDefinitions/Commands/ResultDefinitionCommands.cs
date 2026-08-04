using LIMS.DDD.Service.Domain.SeedWork.Result;
using LIMS.DDD.Service.Domain.StudyTemplateContext.StudyTemplateAggregate;
using LIMS.DDD.Service.Domain.StudyTemplateContext.StudyTemplateAggregate.Entities.ResultDefinitions;
using LIMS.DDD.Service.Domain.StudyTemplateContext.StudyTemplateAggregate.ValueObjects;

namespace LIMS.DDD.Service.Application.StudyTemplates.ResultDefinitions.Commands;

public sealed class ResultDefinitionCommands(IStudyTemplateRepository repository)
{
    public async Task<Result<Guid, Exception>> CreateAsync(
        Guid studyTemplateId,
        CreateResultDefinitionCommand command,
        CancellationToken cancellationToken = default)
    {
        var templateResult = await GetTemplateForChangeAsync(studyTemplateId, cancellationToken);
        if (templateResult.IsFailure) return Result<Guid, Exception>.Failure(templateResult.Error!);

        var newSpecification = Specification.Create(command.MinValue, command.MaxValue);
        if (newSpecification.IsFailure) return Result<Guid, Exception>.Failure(newSpecification.Error!);

        var addResult = templateResult.GetValue()
            .AddResultDefinition(command.ResultInstance, command.Unit, newSpecification.GetValue());
        if (addResult.IsFailure) return Result<Guid, Exception>.Failure(addResult.Error!);

        var saveResult = await SaveChangesAsync(cancellationToken);
        return saveResult.IsFailure
            ? Result<Guid, Exception>.Failure(saveResult.Error!)
            : Result<Guid, Exception>.Success(addResult.GetValue()
                .Id.Value);
    }

    public async Task<Result<Exception>> RemoveAsync(
        Guid studyTemplateId,
        Guid resultId,
        CancellationToken cancellationToken = default)
    {
        var templateResult = await GetTemplateForChangeAsync(studyTemplateId, cancellationToken);
        if (templateResult.IsFailure) return Result<Exception>.Failure(templateResult.Error!);

        var removeResult = templateResult.GetValue()
            .RemoveResultDefinition(new ResultDefinitionId(resultId));
        if (removeResult.IsFailure) return Result<Exception>.Failure(removeResult.Error!);

        return await SaveChangesAsync(cancellationToken);
    }

    public async Task<Result<Exception>> UpdateAsync(
        Guid studyTemplateId,
        Guid resultDefinitionId,
        UpdateResultDefinitionCommand command,
        CancellationToken cancellationToken = default)
    {
        var templateResult = await GetTemplateForChangeAsync(studyTemplateId, cancellationToken);
        if (templateResult.IsFailure) return Result<Exception>.Failure(templateResult.Error!);

        var specification = Specification.Create(command.MinValue, command.MaxValue);
        if (specification.IsFailure) return Result<Exception>.Failure(specification.Error!);

        var updateResult = templateResult.GetValue()
            .UpdateResultDefinition(new ResultDefinitionId(resultDefinitionId), command.ResultInstance, command.Unit,
                specification.GetValue());
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
