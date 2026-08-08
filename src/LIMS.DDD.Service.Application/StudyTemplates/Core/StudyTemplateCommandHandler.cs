using LIMS.DDD.Service.Application.StudyTemplates.Core.Commands;
using LIMS.DDD.Service.Domain.SeedWork.Result;
using LIMS.DDD.Service.Domain.SeedWork.ValueObjects;
using LIMS.DDD.Service.Domain.StudyTemplateContext.StudyTemplateAggregate;
using LIMS.DDD.Service.Domain.StudyTemplateContext.StudyTemplateAggregate.Services;
using LIMS.DDD.Service.Domain.StudyTemplateContext.StudyTemplateAggregate.ValueObjects;

namespace LIMS.DDD.Service.Application.StudyTemplates.Core;

public sealed class StudyTemplateCommandsHandler(
    IStudyTemplateRepository repository,
    StudyTemplateVersioningService domainService)
{
    public async Task<Result<StudyTemplate, Exception>> CreateAsync(
        CreateStudyTemplateCommand command,
        CancellationToken cancellationToken = default)
    {
        var nameResult = Name.Create(command.Name);
        if (nameResult.IsFailure)
        {
            return nameResult.CastFailure<StudyTemplate>();
        }

        var descResult = Description.Create(command.Description);
        if (descResult.IsFailure)
        {
            return descResult.CastFailure<StudyTemplate>();
        }

        var revResult = Revision.Create(command.Revision);
        if (revResult.IsFailure)
        {
            return revResult.CastFailure<StudyTemplate>();
        }

        var createResult = StudyTemplate.Create(nameResult.GetValue(), descResult.GetValue(), revResult.GetValue());
        if (createResult.IsFailure)
        {
            return createResult.CastFailure<StudyTemplate>();
        }

        return await SaveNewAsync(createResult.GetValue(), cancellationToken);
    }

    public async Task<Result<StudyTemplate, Exception>> UpdateAsync(
        Guid id,
        UpdateStudyTemplateCommand command,
        CancellationToken cancellationToken = default)
    {
        var templateResult = await GetTemplateForChangeAsync(id, cancellationToken);
        if (templateResult.IsFailure)
        {
            return templateResult;
        }

        var template = templateResult.GetValue();

        Name? name = null;
        if (command.Name is not null)
        {
            var nameResult = Name.Create(command.Name);
            if (nameResult.IsFailure)
            {
                return nameResult.CastFailure<StudyTemplate>();
            }

            name = nameResult.GetValue();
        }

        Description? description = null;
        if (command.Description is not null)
        {
            var descResult = Description.Create(command.Description);
            if (descResult.IsFailure)
            {
                return descResult.CastFailure<StudyTemplate>();
            }

            description = descResult.GetValue();
        }

        var effectiveName = name ?? template.Name;
        var effectiveDescription = description ?? template.Description;

        var updateResult = template.UpdatePartial(effectiveName, effectiveDescription);
        if (updateResult.IsFailure)
        {
            return updateResult.CastFailure<StudyTemplate>();
        }

        return await SaveChangesAsync(template, cancellationToken);
    }

    public async Task<Result<StudyTemplate, Exception>> ChangeStatusAsync(
        Guid id,
        string statusCommand,
        CancellationToken cancellationToken = default)
    {
        var templateResult = await GetTemplateForChangeAsync(id, cancellationToken);

        if (templateResult.IsFailure)
        {
            return templateResult.CastFailure<StudyTemplate>();
        }

        var template = templateResult.GetValue();

        if (!Status.TryParse(statusCommand, out var newStatus) || newStatus is null)
        {
            return Result<StudyTemplate, Exception>.Failure(
                new InvalidOperationException($"Unknown status '{statusCommand}'."));
        }

        var changeResult = template.ChangeStatus(newStatus);

        if (changeResult.IsFailure)
        {
            return changeResult.CastFailure<StudyTemplate>();
        }

        return await SaveChangesAsync(template, cancellationToken);
    }

    public async Task<Result<None, Exception>> DeleteAsync(
        Guid id,
        CancellationToken cancellationToken = default)
    {
        var templateResult = await GetTemplateForChangeAsync(id, cancellationToken);
        if (templateResult.IsFailure)
        {
            return templateResult.CastFailure<None>();
        }

        var template = templateResult.GetValue();

        var deleteResult = template.Delete();
        if (deleteResult.IsFailure)
        {
            return deleteResult;
        }

        await repository.SaveChangesAsync(cancellationToken);

        return Result<None, Exception>.Success();
    }

    public async Task<Result<Guid, Exception>> CreateRevisionAsync(
        Guid originalId,
        CreateStudyTemplateRevisionCommand command,
        CancellationToken cancellationToken = default)
    {
        var originalResult = await GetTemplateForChangeAsync(originalId, cancellationToken);
        if (originalResult.IsFailure)
        {
            return originalResult.CastFailure<Guid>();
        }

        var revisionResult = Revision.Create(command.NewRevision);
        if (revisionResult.IsFailure)
        {
            return revisionResult.CastFailure<Guid>();
        }

        var createResult = domainService.CreateNewRevision(originalResult.GetValue(), revisionResult.GetValue());
        if (createResult.IsFailure)
        {
            return createResult.CastFailure<Guid>();
        }

        var creationResult = createResult.GetValue();

        var saveResult = await SaveNewAsync(creationResult, cancellationToken);
        return saveResult.IsFailure
            ? saveResult.CastFailure<Guid>()
            : Result<Guid, Exception>.Success(creationResult.Id.Value);
    }

    private async Task<Result<StudyTemplate, Exception>> GetTemplateForChangeAsync(
        Guid id,
        CancellationToken cancellationToken)
    {
        var template = await repository.GetByIdForChangeAsync(new StudyTemplateId(id), cancellationToken);
        return template is null
            ? Result<StudyTemplate, Exception>.Failure(
                new KeyNotFoundException($"StudyTemplate with id {id} not found."))
            : Result<StudyTemplate, Exception>.Success(template);
    }

    private async Task<Result<StudyTemplate, Exception>> SaveNewAsync(
        StudyTemplate template,
        CancellationToken cancellationToken)
    {
        try
        {
            repository.Add(template);
            await repository.SaveChangesAsync(cancellationToken);
            return Result<StudyTemplate, Exception>.Success(template);
        }
        catch (Exception ex)
        {
            return Result<StudyTemplate, Exception>.Failure(new Exception($"Failed to save StudyTemplate: {ex.Message}",
                ex));
        }
    }

    private async Task<Result<StudyTemplate, Exception>> SaveChangesAsync(
        StudyTemplate template,
        CancellationToken cancellationToken)
    {
        try
        {
            await repository.SaveChangesAsync(cancellationToken);
            return Result<StudyTemplate, Exception>.Success(template);
        }
        catch (Exception ex)
        {
            return Result<StudyTemplate, Exception>.Failure(new Exception($"Failed to save StudyTemplate: {ex.Message}",
                ex));
        }
    }
}
