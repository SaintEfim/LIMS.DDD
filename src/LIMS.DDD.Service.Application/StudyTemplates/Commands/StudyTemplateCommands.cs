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
        var descResult = Description.Create(createCommand.Description);
        var revResult = Revision.Create(createCommand.Revision);

        return await nameResult.Bind(name => descResult.Map(desc => (name, desc)))
            .Bind(tuple => revResult.Map(rev => (tuple.name, tuple.desc, rev)))
            .Bind(async tuple =>
            {
                var (name, desc, rev) = tuple;

                var isDuplicate = await repository.ExistsByNameAndRevisionAsync(name, rev, cancellationToken);

                if (isDuplicate)
                {
                    return Result<StudyTemplate, Exception>.Failure(
                        new Exception("Duplicate study template name and revision"));
                }

                return StudyTemplate.Create(name, desc, rev);
            })
            .Bind(async template =>
            {
                repository.Add(template);
                await repository.SaveChangesAsync(cancellationToken);
                return Result<StudyTemplate, Exception>.Success(template);
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

        return await studyTemplate.UpdatePartial(nameResult.Value, descResult.Value)
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

        var canDeleteResult = studyTemplate.EnsureCanBeDeleted();
        if (canDeleteResult.IsFailure) return canDeleteResult;

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

    public async Task<Result<StudyTemplate, Exception>> ApproveAsync(
        Guid id,
        CancellationToken cancellationToken = default)
    {
        var studyTemplateId = new StudyTemplateId(id);
        var studyTemplate = await repository.GetByIdAsync(studyTemplateId, cancellationToken);

        if (studyTemplate is null)
            return Result<StudyTemplate, Exception>.Failure(
                new KeyNotFoundException($"StudyTemplate with id {id} not found."));

        var studyTemplateApproveResult = studyTemplate.Approve();

        if (studyTemplateApproveResult.IsFailure)
            return Result<StudyTemplate, Exception>.Failure(studyTemplateApproveResult.Error!);

        try
        {
            repository.Update(studyTemplate);
            await repository.SaveChangesAsync(cancellationToken);
            return Result<StudyTemplate, Exception>.Success(studyTemplate);
        }
        catch (Exception ex)
        {
            return Result<StudyTemplate, Exception>.Failure(
                new Exception($"Failed to approve study template: {ex.Message}"));
        }
    }

    public async Task<Result<StudyTemplate, Exception>> ArchiveAsync(
        Guid id,
        CancellationToken cancellationToken = default)
    {
        var studyTemplateId = new StudyTemplateId(id);
        var studyTemplate = await repository.GetByIdAsync(studyTemplateId, cancellationToken);

        if (studyTemplate is null)
            return Result<StudyTemplate, Exception>.Failure(
                new KeyNotFoundException($"StudyTemplate with id {id} not found."));

        var studyTemplateArchiveResult = studyTemplate.Archive();

        if (studyTemplateArchiveResult.IsFailure)
            return Result<StudyTemplate, Exception>.Failure(studyTemplateArchiveResult.Error!);

        try
        {
            repository.Update(studyTemplate);
            await repository.SaveChangesAsync(cancellationToken);
            return Result<StudyTemplate, Exception>.Success(studyTemplate);
        }
        catch (Exception ex)
        {
            return Result<StudyTemplate, Exception>.Failure(
                new Exception($"Failed to archive study template: {ex.Message}"));
        }
    }
}
