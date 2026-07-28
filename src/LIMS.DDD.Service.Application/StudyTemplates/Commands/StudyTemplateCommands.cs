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
                    return Result<StudyTemplate, Exception>.Failure(
                        new Exception("Duplicate study template name and revision"));

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
        var studyTemplate = await repository.GetByIdForChangeAsync(studyTemplateId, cancellationToken);

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

    public async Task<Result<StudyTemplate, Exception>> ChangeStatusAsync(
        Guid id,
        string statusCommand,
        CancellationToken cancellationToken = default)
    {
        var studyTemplateId = new StudyTemplateId(id);
        var studyTemplate = await repository.GetByIdForChangeAsync(studyTemplateId, cancellationToken);

        if (studyTemplate is null)
            return Result<StudyTemplate, Exception>.Failure(
                new KeyNotFoundException($"StudyTemplate with id {id} not found."));

        if (!Enum.TryParse<Status>(statusCommand, out var newStatus))
            return Result<StudyTemplate, Exception>.Failure(new KeyNotFoundException("Not found status"));

        var studyTemplateApproveResult = studyTemplate.ChangeStatus(newStatus);

        if (studyTemplateApproveResult.IsFailure)
            return Result<StudyTemplate, Exception>.Failure(studyTemplateApproveResult.Error!);

        try
        {
            await repository.SaveChangesAsync(cancellationToken);
            return Result<StudyTemplate, Exception>.Success(studyTemplate);
        }
        catch (Exception ex)
        {
            return Result<StudyTemplate, Exception>.Failure(
                new Exception($"Failed to approve study template: {ex.Message}"));
        }
    }

    public async Task<Result<Guid, Exception>> CreateRevisionAsync(
        Guid originalStudyTemplateId,
        CreateStudyTemplateRevisionCommand command,
        CancellationToken cancellationToken = default)
    {
        var original = await repository.GetByIdForChangeAsync(new StudyTemplateId(originalStudyTemplateId),
            cancellationToken);

        if (original is null)
            return Result<Guid, Exception>.Failure(
                new KeyNotFoundException($"StudyTemplate with id {originalStudyTemplateId} not found."));

        var revisionResult = Revision.Create(command.NewRevision);
        if (revisionResult.IsFailure) return Result<Guid, Exception>.Failure(revisionResult.Error!);

        var isDuplicate = await repository.ExistsByNameAndRevisionAsync(
            original.Name, revisionResult.Value, cancellationToken);

        if (isDuplicate)
            return Result<Guid, Exception>.Failure(new InvalidOperationException(
                $"StudyTemplate with name '{original.Name.Value}' and revision '{command.NewRevision}' already exists."));

        return await StudyTemplateVersioningService.CreateNewRevisionAsync(original, revisionResult.Value)
            .Bind(async template =>
            {
                try
                {
                    repository.Add(template);
                    await repository.SaveChangesAsync(cancellationToken);
                    return Result<Guid, Exception>.Success(template.Id.Value);
                }
                catch (Exception ex)
                {
                    return Result<Guid, Exception>.Failure(
                        new Exception($"Failed to create revision: {ex.Message}", ex));
                }
            });
    }
}
