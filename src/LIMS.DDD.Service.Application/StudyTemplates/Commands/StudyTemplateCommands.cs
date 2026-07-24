using LIMS.DDD.Service.Domain;
using LIMS.DDD.Service.Domain.SeedWork.Result;
using LIMS.DDD.Service.Domain.StudyTemplateAggregate;

namespace LIMS.DDD.Service.Application.StudyTemplates.Commands;

public sealed class StudyTemplateCommands(IStudyTemplateRepository repository)
{
    public async Task<Result<StudyTemplate, Exception>> CreateAsync(
        CreateStudyTemplateCommand createCommand,
        CancellationToken cancellationToken = default)
    {
        var nameResult = Name.Create(createCommand.Name);
        if (nameResult is { IsFailure: true, Error: not null })
        {
            return Result<StudyTemplate, Exception>.Failure(nameResult.Error);
        }

        var descriptionResult = Description.Create(createCommand.Description);
        if (descriptionResult is { IsFailure: true, Error: not null })
        {
            return Result<StudyTemplate, Exception>.Failure(descriptionResult.Error);
        }

        var revisionResult = Revision.Create(createCommand.Description);
        if (revisionResult is { IsFailure: true, Error: not null })
        {
            return Result<StudyTemplate, Exception>.Failure(revisionResult.Error);
        }

        var createResult = StudyTemplate.Create(nameResult.Value, descriptionResult.Value, revisionResult.Value);

        return await createResult.Bind(async template =>
        {
            try
            {
                repository.Add(template);
                await repository.SaveChangesAsync(cancellationToken);
                return createResult;
            }
            catch (Exception ex)
            {
                return Result<StudyTemplate, Exception>.Failure(
                    new Exception($"Failed to save study template: {ex.Message}"));
            }
        });
    }

    public async Task<Result<Exception>> UpdateAsync(
        Guid id,
        UpdateStudyTemplateCommand updateCommand,
        CancellationToken cancellationToken = default)
    {
        var studyTemplateId = new StudyTemplateId(id);
        var studyTemplate = await repository.GetByIdAsync(studyTemplateId, cancellationToken);

        if (studyTemplate is null)
            return Result<Exception>.Failure(new KeyNotFoundException($"StudyTemplate with id {id} not found."));

        Name? name = null;
        if (updateCommand.Name is not null)
        {
            var nameResult = Name.Create(updateCommand.Name);

            if (nameResult is { IsFailure: true, Error: not null })
            {
                return Result<Exception>.Failure(nameResult.Error);
            }

            name = nameResult.Value;
        }

        Description? description = null;
        if (updateCommand.Description is not null)
        {
            var descriptionResult = Description.Create(updateCommand.Name);

            if (descriptionResult is { IsFailure: true, Error: not null })
            {
                return Result<Exception>.Failure(descriptionResult.Error);
            }

            description = descriptionResult.Value;
        }

        Revision? rev = null;
        if (updateCommand.Revision is not null)
        {
            var revisionResult = Revision.Create(updateCommand.Revision);

            if (revisionResult is { IsFailure: true, Error: not null })
            {
                return Result<Exception>.Failure(revisionResult.Error);
            }

            rev = revisionResult.Value;
        }

        var updateResult = studyTemplate.UpdatePartial(name, description, rev);
        if (updateResult.IsFailure) return updateResult;

        try
        {
            repository.Update(studyTemplate);
            await repository.SaveChangesAsync(cancellationToken);
            return Result<Exception>.Success();
        }
        catch (Exception e)
        {
            return Result<Exception>.Failure(e);
        }
    }

    public async Task<Result<Exception>> DeleteAsync(
        Guid id,
        CancellationToken cancellationToken = default)
    {
        var studyTemplate = await repository.GetByIdAsync(new StudyTemplateId(id), cancellationToken);

        if (studyTemplate is null)
            return Result<Exception>.Failure(new KeyNotFoundException($"StudyTemplate with id {id} not found."));

        try
        {
            repository.Remove(studyTemplate);
            await repository.SaveChangesAsync(cancellationToken);
            return Result<Exception>.Success();
        }
        catch (Exception e)
        {
            return Result<Exception>.Failure(e);
        }
    }

    public async Task<Result<Exception>> ChangeStatusAsync(
        Guid id,
        ChangeStatusCommand command,
        CancellationToken cancellationToken = default)
    {
        var studyTemplate = await repository.GetByIdAsync(new StudyTemplateId(id), cancellationToken);

        if (studyTemplate is null)
            return Result<Exception>.Failure(new KeyNotFoundException($"StudyTemplate with id {id} not found."));

        if (!Enum.TryParse<Status>(command.Status, ignoreCase: true, out var newStatus))
            return Result<Exception>.Failure(new ArgumentException($"Invalid status value: {command.Status}"));

        studyTemplate.ChangeStatus(newStatus);

        try
        {
            repository.Update(studyTemplate);
            await repository.SaveChangesAsync(cancellationToken);
            return Result<Exception>.Success();
        }
        catch (Exception e)
        {
            return Result<Exception>.Failure(e);
        }
    }
}
