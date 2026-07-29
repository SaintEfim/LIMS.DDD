using LIMS.DDD.Service.Domain.SeedWork.Result;
using LIMS.DDD.Service.Domain.StudyTemplateAggregate;
using LIMS.DDD.Service.Domain.StudyTemplateAggregate.Enums;
using LIMS.DDD.Service.Domain.StudyTemplateAggregate.Ids;
using LIMS.DDD.Service.Domain.StudyTemplateAggregate.Services;
using LIMS.DDD.Service.Domain.StudyTemplateAggregate.ValueObjects;

namespace LIMS.DDD.Service.Application.StudyTemplates.Commands;

public sealed class StudyTemplateCommands(IStudyTemplateRepository repository)
{
    public async Task<Result<StudyTemplate, Exception>> CreateAsync(
        CreateStudyTemplateCommand command,
        CancellationToken cancellationToken = default)
    {
        var nameResult = Name.Create(command.Name);
        var descriptionResult = Description.Create(command.Description);
        var revisionResult = Revision.Create(command.Revision);

        return await nameResult.Bind(name => descriptionResult.Map(description => (name, description)))
            .Bind(data => revisionResult.Map(revision => (data.name, data.description, revision)))
            .Bind(async data =>
            {
                var duplicateResult = await CheckDuplicateAsync(data.name, data.revision, cancellationToken);

                return duplicateResult.IsFailure
                    ? Result<(Name name, Description description, Revision revision), Exception>.Failure(duplicateResult
                        .Error!)
                    : Result<(Name name, Description description, Revision revision), Exception>.Success(data);
            })
            .Bind(data => StudyTemplate.Create(data.name, data.description, data.revision))
            .Bind(async template =>
            {
                repository.Add(template);

                return await SaveAsync(template, cancellationToken);
            });
    }

    public async Task<Result<StudyTemplate, Exception>> UpdateAsync(
        Guid id,
        UpdateStudyTemplateCommand command,
        CancellationToken cancellationToken = default)
    {
        var templateResult = await GetStudyTemplateAsync(id, cancellationToken);

        if (templateResult.IsFailure) return templateResult;

        var template = templateResult.Value!;

        var nameResult = command.Name is null
            ? Result<Name?, Exception>.Success(null)
            : Name.Create(command.Name)
                .Map(x => (Name?) x);

        var descriptionResult = command.Description is null
            ? Result<Description?, Exception>.Success(null)
            : Description.Create(command.Description)
                .Map(x => (Description?) x);

        return await nameResult.Bind(name => descriptionResult.Map(description => (name, description)))
            .Bind(async data =>
            {
                if (data.name is null || data.name == template.Name)
                {
                    return Result<(Name? name, Description? description), Exception>.Success(data);
                }

                var duplicateResult = await CheckDuplicateAsync(data.name.Value, template.Revision, cancellationToken);

                return duplicateResult.IsFailure
                    ? Result<(Name? name, Description? description), Exception>.Failure(duplicateResult.Error!)
                    : Result<(Name? name, Description? description), Exception>.Success(data);
            })
            .Bind(data => template.UpdatePartial(data.name ?? template.Name, data.description ?? template.Description))
            .Bind(async updatedTemplate => await SaveAsync(updatedTemplate, cancellationToken));
    }

    public async Task<Result<StudyTemplate, Exception>> ChangeStatusAsync(
        Guid id,
        string statusCommand,
        CancellationToken cancellationToken = default)
    {
        var templateResult = await GetStudyTemplateAsync(id, cancellationToken);

        if (templateResult.IsFailure) return templateResult;

        if (!Enum.TryParse<Status>(statusCommand, true, out var status))
        {
            return Result<StudyTemplate, Exception>.Failure(
                new InvalidOperationException($"Unknown status '{statusCommand}'."));
        }

        var template = templateResult.Value!;

        var resultChangeStatus = template.ChangeStatus(status);
        if (resultChangeStatus.IsFailure) Result<StudyTemplate, Exception>.Failure(resultChangeStatus.Error!);

        return await SaveAsync(template, cancellationToken);
    }

    public async Task<Result<Guid, Exception>> CreateRevisionAsync(
        Guid originalId,
        CreateStudyTemplateRevisionCommand command,
        CancellationToken cancellationToken = default)
    {
        var originalResult = await GetStudyTemplateAsync(originalId, cancellationToken);

        if (originalResult.IsFailure)
        {
            return Result<Guid, Exception>.Failure(originalResult.Error!);
        }

        var original = originalResult.Value!;

        return await Revision.Create(command.NewRevision)
            .Bind(async revision =>
            {
                var duplicateResult = await CheckDuplicateAsync(original.Name, revision, cancellationToken);

                return duplicateResult.IsFailure
                    ? Result<Revision, Exception>.Failure(duplicateResult.Error!)
                    : Result<Revision, Exception>.Success(revision);
            })
            .Bind(revision => StudyTemplateVersioningService.CreateNewRevisionAsync(original, revision))
            .Bind(async template =>
            {
                repository.Add(template);

                var saveResult = await SaveAsync(template, cancellationToken);

                return saveResult.IsFailure
                    ? Result<Guid, Exception>.Failure(saveResult.Error!)
                    : Result<Guid, Exception>.Success(template.Id.Value);
            });
    }

    private async Task<Result<StudyTemplate, Exception>> GetStudyTemplateAsync(
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
            ? Result<bool, Exception>.Failure(new InvalidOperationException($"StudyTemplate with name '{name.Value}' " +
                                                                            $"and revision '{revision.Value}' already exists."))
            : Result<bool, Exception>.Success(exists);
    }

    private async Task<Result<StudyTemplate, Exception>> SaveAsync(
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
            return Result<StudyTemplate, Exception>.Failure(new Exception("Failed to save StudyTemplate.", ex));
        }
    }
}
