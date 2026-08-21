using Application.SeedWork;
using Domain.SeedWork;
using Domain.SeedWork.Result;
using Domain.SeedWork.ValueObjects;
using LIMS.Service.Methodologies.Application.StudyTemplates.ResultDefinitions.Commands;
using LIMS.Service.Methodologies.Domain.StudyTemplateAggregate;
using LIMS.Service.Methodologies.Domain.StudyTemplateAggregate.Entities.ResultDefinitions;
using LIMS.Service.Methodologies.Domain.UnitSnapshots;

namespace LIMS.Service.Methodologies.Application.StudyTemplates.ResultDefinitions;

public sealed class ResultDefinitionCommandsHandler(
    IStudyTemplateRepository repository,
    IUnitSnapshotRepository unitSnapshotRepository,
    IUnitOfWork unitOfWork) : ICommandsHandler
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
            return new KeyNotFoundException($"Unit with id {command.UnitId} not found.");
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
            : addResult.GetValue()
                .Id.Value;
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
                return new KeyNotFoundException($"Unit with id {command.UnitId} not found.");
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
            ? new KeyNotFoundException($"StudyTemplate with id {studyTemplateId} not found.")
            : template;
    }

    private async Task<Result<None, Exception>> SaveChangesAsync(
        CancellationToken cancellationToken = default)
    {
        try
        {
            await unitOfWork.SaveChangesAsync(cancellationToken);
            return new None();
        }
        catch (Exception ex)
        {
            return new Exception($"Failed to save changes: {ex.Message}", ex);
        }
    }
}
