using LIMS.DDD.Service.Application.StudyTemplates.ResultDefinitions.Commands;
using LIMS.DDD.Service.Domain.SeedWork;
using LIMS.DDD.Service.Domain.SeedWork.Result;
using LIMS.DDD.Service.Domain.SeedWork.Snapshots;
using LIMS.DDD.Service.Domain.SeedWork.ValueObjects;
using LIMS.DDD.Service.Domain.StudyTemplateContext.StudyTemplateAggregate;
using LIMS.DDD.Service.Domain.StudyTemplateContext.StudyTemplateAggregate.Entities.ResultDefinitions;

namespace LIMS.DDD.Service.Application.StudyTemplates.ResultDefinitions;

public sealed class ResultDefinitionCommandsHandler(
    IStudyTemplateRepository repository,
    IUnitSnapshotRepository unitSnapshotRepository,
    IUnitOfWork unitOfWork)
{
    public async Task<Result<Guid, Exception>> CreateAsync(
        Guid studyTemplateId,
        CreateResultDefinitionCommand command,
        CancellationToken cancellationToken = default)
    {
        var templateResult = await GetTemplateForChangeAsync(studyTemplateId, cancellationToken);
        if (templateResult.IsFailure)
        {
            return templateResult.CastFailure<Guid>();
        }

        var newSpecification = Specification.Create(command.MinValue, command.MaxValue);
        if (newSpecification.IsFailure)
        {
            return newSpecification.CastFailure<Guid>();
        }

        var unit = await unitSnapshotRepository.GetByIdAsync(new UnitId(command.UnitId), cancellationToken);
        if (unit is null)
        {
            return Result<Guid, Exception>.Failure(
                new KeyNotFoundException($"Unit with id {command.UnitId} not found."));
        }

        var addResult = templateResult.GetValue()
            .AddResultDefinition(command.ResultInstance, unit.Id, newSpecification.GetValue());
        if (addResult.IsFailure)
        {
            return addResult.CastFailure<Guid>();
        }

        var saveResult = await SaveChangesAsync(cancellationToken);
        return saveResult.IsFailure
            ? saveResult.CastFailure<Guid>()
            : Result<Guid, Exception>.Success(addResult.GetValue()
                .Id.Value);
    }

    public async Task<Result<None, Exception>> RemoveAsync(
        Guid studyTemplateId,
        Guid resultId,
        CancellationToken cancellationToken = default)
    {
        var templateResult = await GetTemplateForChangeAsync(studyTemplateId, cancellationToken);
        if (templateResult.IsFailure)
        {
            return templateResult.CastFailure<None>();
        }

        var removeResult = templateResult.GetValue()
            .RemoveResultDefinition(new ResultDefinitionId(resultId));
        if (removeResult.IsFailure)
        {
            return removeResult.CastFailure<None>();
        }

        return await SaveChangesAsync(cancellationToken);
    }

    public async Task<Result<None, Exception>> UpdateAsync(
        Guid studyTemplateId,
        Guid resultDefinitionId,
        UpdateResultDefinitionCommand command,
        CancellationToken cancellationToken = default)
    {
        var templateResult = await GetTemplateForChangeAsync(studyTemplateId, cancellationToken);
        if (templateResult.IsFailure)
        {
            return templateResult.CastFailure<None>();
        }

        UnitSnapshot? unit = null;
        if (command.UnitId is not null)
        {
            unit = await unitSnapshotRepository.GetByIdAsync(new UnitId(command.UnitId.Value), cancellationToken);
            if (unit is null)
            {
                return Result<None, Exception>.Failure(
                    new KeyNotFoundException($"Unit with id {command.UnitId} not found."));
            }
        }

        var updateResult = templateResult.GetValue()
            .UpdateResultDefinition(new ResultDefinitionId(resultDefinitionId), command.ResultInstance, unit?.Id,
                command.MinValue, command.MaxValue);
        if (updateResult.IsFailure)
        {
            return updateResult.CastFailure<None>();
        }

        return await SaveChangesAsync(cancellationToken);
    }

    private async Task<Result<StudyTemplate, Exception>> GetTemplateForChangeAsync(
        Guid studyTemplateId,
        CancellationToken cancellationToken = default)
    {
        var template = await repository.GetByIdForChangeAsync(new StudyTemplateId(studyTemplateId), cancellationToken);
        return template is null
            ? Result<StudyTemplate, Exception>.Failure(
                new KeyNotFoundException($"StudyTemplate with id {studyTemplateId} not found."))
            : Result<StudyTemplate, Exception>.Success(template);
    }

    private async Task<Result<None, Exception>> SaveChangesAsync(
        CancellationToken cancellationToken = default)
    {
        try
        {
            await unitOfWork.SaveChangesAsync(cancellationToken);
            return Result<None, Exception>.Success();
        }
        catch (Exception ex)
        {
            return Result<None, Exception>.Failure(new Exception($"Failed to save changes: {ex.Message}", ex));
        }
    }
}
