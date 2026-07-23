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
        var studyTemplate = StudyTemplate.Create(new Name(createCommand.Name),
            new Description(createCommand.Description), new Revision(createCommand.Revision));

        return await studyTemplate.OnSuccess(async x =>
        {
            try
            {
                repository.Add(x);
                await repository.SaveChangesAsync(cancellationToken);
            }
            catch (Exception e)
            {
                Result<StudyTemplate, Exception>.Failure(e);
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

        Name? name = updateCommand.Name is not null ? new Name(updateCommand.Name) : null;
        Description? desc = updateCommand.Description is not null ? new Description(updateCommand.Description) : null;
        Revision? rev = updateCommand.Revision is not null ? new Revision(updateCommand.Revision) : null;

        studyTemplate.UpdatePartial(name, desc, rev)
            .OnFailure(x => Result<Exception>.Failure(x));

        try
        {
            repository.Update(studyTemplate);
            await repository.SaveChangesAsync(cancellationToken);
        }
        catch (Exception e)
        {
            Result<StudyTemplate, Exception>.Failure(e);
        }

        return Result<Exception>.Success();
    }

    public async Task<Result<Exception>> DeleteAsync(
        Guid id,
        CancellationToken cancellationToken = default)
    {
        var studyTemplate = await repository.GetByIdAsync(new StudyTemplateId(id), cancellationToken);

        if (studyTemplate is null)
            return Result<Exception>.Failure(new KeyNotFoundException($"StudyTemplate with id {id} not found."));

        repository.Remove(studyTemplate);
        await repository.SaveChangesAsync(cancellationToken);

        return Result<Exception>.Success();
    }

    public async Task ChangeStatusAsync(
        Guid id,
        ChangeStatusCommand command,
        CancellationToken cancellationToken = default)
    {
        var studyTemplate = await repository.GetByIdAsync(new StudyTemplateId(id), cancellationToken);

        if (!Enum.TryParse<Status>(command.Status, ignoreCase: true, out var newStatus))
            throw new ArgumentException($"Invalid status value: {command.Status}");

        studyTemplate.ChangeStatus(newStatus);

        repository.Update(studyTemplate);
        await repository.SaveChangesAsync(cancellationToken);
    }
}
