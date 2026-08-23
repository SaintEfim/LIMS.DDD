using Application.SeedWork;
using Application.SeedWork.Errors;
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
    public async Task<Result<Guid, ApplicationError>> CreateAsync(
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
            return new DomainRuleViolation(newSpecification.GetError());
        }

        var unit = await unitSnapshotRepository.GetByIdAsync(new UnitId(command.UnitId), cancellationToken);
        if (unit is null)
        {
            return new NotFoundError($"Unit with id '{command.UnitId}' not found.");
        }

        var addResult = templateResult.GetValue()
            .AddResultDefinition(command.ResultInstance, unit.Id, newSpecification.GetValue());
        if (addResult.IsFailure)
        {
            return new DomainRuleViolation(addResult.GetError());
        }

        var saveResult = await SaveChangesAsync(cancellationToken);
        return saveResult.IsFailure
            ? saveResult.CastFailure<Guid>()
            : addResult.GetValue()
                .Id.Value;
    }

    public async Task<Result<None, ApplicationError>> RemoveAsync(
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
            return new DomainRuleViolation(removeResult.GetError());
        }

        return await SaveChangesAsync(cancellationToken);
    }

    public async Task<Result<None, ApplicationError>> UpdateAsync(
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
                return new NotFoundError($"Unit with id '{command.UnitId.Value}' not found.");
            }
        }

        var updateResult = templateResult.GetValue()
            .UpdateResultDefinition(new ResultDefinitionId(resultDefinitionId), command.ResultInstance, unit?.Id,
                command.MinValue, command.MaxValue);
        if (updateResult.IsFailure)
        {
            return new DomainRuleViolation(updateResult.GetError());
        }

        return await SaveChangesAsync(cancellationToken);
    }

    private async Task<Result<StudyTemplate, ApplicationError>> GetTemplateForChangeAsync(
        Guid studyTemplateId,
        CancellationToken cancellationToken = default)
    {
        var template = await repository.GetByIdForChangeAsync(new StudyTemplateId(studyTemplateId), cancellationToken);
        if (template is null)
        {
            return new NotFoundError($"Study template with id '{studyTemplateId}' not found.");
        }

        return template;
    }

    private async Task<Result<None, ApplicationError>> SaveChangesAsync(
        CancellationToken cancellationToken = default)
    {
        try
        {
            await unitOfWork.SaveChangesAsync(cancellationToken);
            return new None();
        }
        catch (Exception ex)
        {
            return new PersistenceError($"Failed to save changes: {ex.Message}");
        }
    }
}
