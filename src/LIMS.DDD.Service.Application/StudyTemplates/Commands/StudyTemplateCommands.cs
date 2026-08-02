using LIMS.DDD.Service.Domain.SeedWork.Result;
using LIMS.DDD.Service.Domain.SeedWork.ValueObjects;
using LIMS.DDD.Service.Domain.StudyTemplateContext.StudyTemplateAggregate;
using LIMS.DDD.Service.Domain.StudyTemplateContext.StudyTemplateAggregate.Ids;
using LIMS.DDD.Service.Domain.StudyTemplateContext.StudyTemplateAggregate.Services;
using LIMS.DDD.Service.Domain.StudyTemplateContext.StudyTemplateAggregate.ValueObjects;

namespace LIMS.DDD.Service.Application.StudyTemplates.Commands;

public sealed class StudyTemplateCommands(IStudyTemplateRepository repository)
{
    public async Task<Result<StudyTemplate, Exception>> CreateAsync(
        CreateStudyTemplateCommand command,
        CancellationToken cancellationToken = default)
    {
        var nameResult = Name.Create(command.Name);
        if (nameResult.IsFailure) return Result<StudyTemplate, Exception>.Failure(nameResult.Error!);

        var descResult = Description.Create(command.Description);
        if (descResult.IsFailure) return Result<StudyTemplate, Exception>.Failure(descResult.Error!);

            var revResult = Revision.Create(command.Revision);
            if (revResult.IsFailure) return Result<StudyTemplate, Exception>.Failure(revResult.Error!);

            var duplicateResult = await CheckDuplicateAsync(nameResult.GetValue(), revResult.GetValue(), cancellationToken);
            if (duplicateResult.IsFailure) return Result<StudyTemplate, Exception>.Failure(duplicateResult.Error!);

        var createResult = StudyTemplate.Create(nameResult.GetValue(), descResult.GetValue(), revResult.GetValue());
        if (createResult.IsFailure) return Result<StudyTemplate, Exception>.Failure(createResult.Error!);

        return await SaveNewAsync(createResult.GetValue(), cancellationToken);
    }

    public async Task<Result<StudyTemplate, Exception>> UpdateAsync(
        Guid id,
        UpdateStudyTemplateCommand command,
        CancellationToken cancellationToken = default)
    {
        var templateResult = await GetTemplateForChangeAsync(id, cancellationToken);
        if (templateResult.IsFailure) return templateResult;

        var template = templateResult.GetValue();

        Name? name = null;
        if (command.Name is not null)
        {
            var nameResult = Name.Create(command.Name);
            if (nameResult.IsFailure) return Result<StudyTemplate, Exception>.Failure(nameResult.Error!);
            name = nameResult.GetValue();
        }

        Description? description = null;
        if (command.Description is not null)
        {
            var descResult = Description.Create(command.Description);
            if (descResult.IsFailure) return Result<StudyTemplate, Exception>.Failure(descResult.Error!);
            description = descResult.GetValue();
        }

        if (name is not null && name != template.Name)
        {
            var duplicateResult = await CheckDuplicateAsync(name, template.Revision, cancellationToken);
            if (duplicateResult.IsFailure) return Result<StudyTemplate, Exception>.Failure(duplicateResult.Error!);
        }

        var effectiveName = name ?? template.Name;
        var effectiveDescription = description ?? template.Description;

        var updateResult = template.UpdatePartial(effectiveName, effectiveDescription);
        if (updateResult.IsFailure) return Result<StudyTemplate, Exception>.Failure(updateResult.Error!);

        return await SaveChangesAsync(template, cancellationToken);
    }

    public async Task<Result<StudyTemplate, Exception>> ChangeStatusAsync(
        Guid id,
        string statusCommand,
        CancellationToken cancellationToken = default)
    {
        var templateResult = await GetTemplateForChangeAsync(id, cancellationToken);

        if (templateResult.IsFailure) return Result<StudyTemplate, Exception>.Failure(templateResult.Error!);

        var template = templateResult.GetValue();

        if (!Status.TryParse(statusCommand, out var newStatus))
        {
            return Result<StudyTemplate, Exception>.Failure(
                new InvalidOperationException($"Unknown status '{statusCommand}'."));
        }

        var changeResult = template.ChangeStatus(newStatus);

        if (changeResult.IsFailure) return Result<StudyTemplate, Exception>.Failure(changeResult.Error!);

        return await SaveChangesAsync(template, cancellationToken);
    }

    public async Task<Result<Guid, Exception>> CreateRevisionAsync(
        Guid originalId,
        CreateStudyTemplateRevisionCommand command,
        CancellationToken cancellationToken = default)
    {
        var originalResult = await GetTemplateForChangeAsync(originalId, cancellationToken);
        if (originalResult.IsFailure) return Result<Guid, Exception>.Failure(originalResult.Error!);

        var revisionResult = Revision.Create(command.NewRevision);
        if (revisionResult.IsFailure) return Result<Guid, Exception>.Failure(revisionResult.Error!);

        var duplicateResult = await CheckDuplicateAsync(
            originalResult.GetValue().Name, revisionResult.GetValue(), cancellationToken);
        if (duplicateResult.IsFailure) return Result<Guid, Exception>.Failure(duplicateResult.Error!);

        var createResult =
            StudyTemplateVersioningService.CreateNewRevisionAsync(originalResult.GetValue(), revisionResult.GetValue());
        if (createResult.IsFailure) return Result<Guid, Exception>.Failure(createResult.Error!);

        var saveResult = await SaveNewAsync(createResult.GetValue(), cancellationToken);
        return saveResult.IsFailure
            ? Result<Guid, Exception>.Failure(saveResult.Error!)
            : Result<Guid, Exception>.Success(createResult.GetValue().Id.Value);
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

    private async Task<Result<bool, Exception>> CheckDuplicateAsync(
        Name name,
        Revision revision,
        CancellationToken cancellationToken)
    {
        var exists = await repository.ExistsByNameAndRevisionAsync(name, revision, cancellationToken);
        return exists
            ? Result<bool, Exception>.Failure(new InvalidOperationException(
                $"StudyTemplate with name '{name.Value}' and revision '{revision.Value}' already exists."))
            : Result<bool, Exception>.Success(false);
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
            return Result<StudyTemplate, Exception>.Failure(new Exception("Failed to save StudyTemplate.", ex));
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
