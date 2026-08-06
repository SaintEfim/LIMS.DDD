using LIMS.DDD.Service.Application.StudyTemplates.ResultDefinitions.Commands;
using LIMS.DDD.Service.Domain.SeedWork.Result;
using LIMS.DDD.Service.Domain.SeedWork.ValueObjects;
using LIMS.DDD.Service.Domain.StudyTemplateContext.StudyTemplateAggregate;
using LIMS.DDD.Service.Domain.StudyTemplateContext.StudyTemplateAggregate.Entities.ResultDefinitions;

namespace LIMS.DDD.Service.Application.StudyTemplates.ResultDefinitions;

public sealed class ResultDefinitionCommands(IStudyTemplateRepository repository)
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

        var addResult = templateResult.GetValue()
            .AddResultDefinition(command.ResultInstance, command.Unit, newSpecification.GetValue());
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

    public async Task<Result<UnitEmpty, Exception>> RemoveAsync(
        Guid studyTemplateId,
        Guid resultId,
        CancellationToken cancellationToken = default)
    {
        var templateResult = await GetTemplateForChangeAsync(studyTemplateId, cancellationToken);
        if (templateResult.IsFailure)
        {
            return templateResult.CastFailure<UnitEmpty>();
        }

        var removeResult = templateResult.GetValue()
            .RemoveResultDefinition(new ResultDefinitionId(resultId));
        if (removeResult.IsFailure)
        {
            return removeResult.CastFailure<UnitEmpty>();
        }

        return await SaveChangesAsync(cancellationToken);
    }

    public async Task<Result<UnitEmpty, Exception>> UpdateAsync(
        Guid studyTemplateId,
        Guid resultDefinitionId,
        UpdateResultDefinitionCommand command,
        CancellationToken cancellationToken = default)
    {
        var templateResult = await GetTemplateForChangeAsync(studyTemplateId, cancellationToken);
        if (templateResult.IsFailure)
        {
            return templateResult.CastFailure<UnitEmpty>();
        }

        var updateResult = templateResult.GetValue()
            .UpdateResultDefinition(new ResultDefinitionId(resultDefinitionId), command.ResultInstance, command.Unit,
                command.MinValue, command.MaxValue);
        if (updateResult.IsFailure)
        {
            return updateResult.CastFailure<UnitEmpty>();
        }

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

    private async Task<Result<UnitEmpty, Exception>> SaveChangesAsync(
        CancellationToken cancellationToken)
    {
        try
        {
            await repository.SaveChangesAsync(cancellationToken);
            return Result<UnitEmpty, Exception>.Success(new UnitEmpty());
        }
        catch (Exception ex)
        {
            return Result<UnitEmpty, Exception>.Failure(new Exception($"Failed to save changes: {ex.Message}", ex));
        }
    }
}
