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
        return await Name.Create(createCommand.Name)
            .Bind(name => Description.Create(createCommand.Description)
                .Map(desc => (name, desc)))
            .Bind(tuple => Revision.Create(createCommand.Revision)
                .Map(rev => (tuple.name, tuple.desc, rev)))
            .Bind(tuple => StudyTemplate.Create(tuple.name, tuple.desc, tuple.rev))
            .Bind(async template =>
            {
                try
                {
                    repository.Add(template);
                    await repository.SaveChangesAsync(cancellationToken);
                    return Result<StudyTemplate, Exception>.Success(template);
                }
                catch (Exception ex)
                {
                    return Result<StudyTemplate, Exception>.Failure(
                        new Exception($"Failed to save study template: {ex.Message}"));
                }
            });
    }

    public async Task<Result<StudyTemplate, Exception>> UpdateAsync(
        Guid id,
        UpdateStudyTemplateCommand updateCommand,
        CancellationToken cancellationToken = default)
    {
        var studyTemplateId = new StudyTemplateId(id);
        var studyTemplate = await repository.GetByIdAsync(studyTemplateId, cancellationToken);

        if (studyTemplate is null)
            return Result<StudyTemplate, Exception>.Failure(
                new KeyNotFoundException($"StudyTemplate with id {id} not found."));

        var nameResult = updateCommand.Name is null
            ? Result<Name?, Exception>.Success(null)
            : Name.Create(updateCommand.Name)
                .Map(n => (Name?) n);

        var descResult = updateCommand.Description is null
            ? Result<Description?, Exception>.Success(null)
            : Description.Create(updateCommand.Description)
                .Map(d => (Description?) d);

        var revResult = updateCommand.Revision is null
            ? Result<Revision?, Exception>.Success(null)
            : Revision.Create(updateCommand.Revision)
                .Map(r => (Revision?) r);

        return await studyTemplate.UpdatePartial(nameResult.Value, descResult.Value, revResult.Value)
            .Bind(async template =>
            {
                try
                {
                    repository.Update(studyTemplate);
                    await repository.SaveChangesAsync(cancellationToken);

                    return Result<StudyTemplate, Exception>.Success(template);
                }
                catch (Exception ex)
                {
                    return Result<StudyTemplate, Exception>.Failure(
                        new Exception($"Failed to save study template: {ex.Message}"));
                }
            });
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
