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
        var createResult = StudyTemplate.Create(new Name(createCommand.Name),
            new Description(createCommand.Description), new Revision(createCommand.Revision));

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

        Name? name = updateCommand.Name is not null ? new Name(updateCommand.Name) : null;
        Description? desc = updateCommand.Description is not null ? new Description(updateCommand.Description) : null;
        Revision? rev = updateCommand.Revision is not null ? new Revision(updateCommand.Revision) : null;

        var updateResult = studyTemplate.UpdatePartial(name, desc, rev);
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
