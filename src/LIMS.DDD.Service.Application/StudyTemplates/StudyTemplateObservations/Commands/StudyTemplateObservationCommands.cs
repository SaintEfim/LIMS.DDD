using LIMS.DDD.Service.Domain;
using LIMS.DDD.Service.Domain.SeedWork.Result;
using LIMS.DDD.Service.Domain.StudyTemplateAggregate;
using LIMS.DDD.Service.Domain.StudyTemplateAggregate.StudyTemplateObservations;

namespace LIMS.DDD.Service.Application.StudyTemplates.StudyTemplateObservations.Commands;

public sealed class StudyTemplateObservationCommands(IStudyTemplateRepository repository)
{
    public async Task<Result<Guid, Exception>> AddAddStudyTemplateObservationAsync(
        Guid studyTemplateId,
        CreateStudyTemplateObservationCommand command,
        CancellationToken cancellationToken = default)
    {
        var studyTemplate = await repository.GetByIdForChangeAsync(
            new StudyTemplateId(studyTemplateId), cancellationToken);

        if (studyTemplate is null)
        {
            return Result<Guid, Exception>.Failure(
                new KeyNotFoundException($"StudyTemplate with id {studyTemplateId} not found."));
        }

        return await Name.Create(command.Name)
            .Bind(name => Description.Create(command.Description)
                .Map(description => (name, description)))
            .Bind(tuple => AliasName.Create(command.AliasName)
                .Map(aliasName => (tuple.name, tuple.description, aliasName)))
            .Bind(tuple => studyTemplate.AddStudyTemplateObservation(tuple.name, tuple.description, tuple.aliasName,
                new Specification(command.MinValue, command.MaxValue)))
            .Bind(async result =>
            {
                try
                {
                    await repository.SaveChangesAsync(cancellationToken);
                    return Result<Guid, Exception>.Success(result.Id.Value);
                }
                catch (Exception ex)
                {
                    return Result<Guid, Exception>.Failure(new Exception($"Failed to save parameter: {ex.Message}",
                        ex));
                }
            });
    }

    public async Task<Result<Exception>> RemoveRemoveStudyTemplateObservationAsync(
        Guid studyTemplateId,
        Guid parameterId,
        CancellationToken cancellationToken = default)
    {
        var studyTemplate = await repository.GetByIdForChangeAsync(
            new StudyTemplateId(studyTemplateId), cancellationToken);

        if (studyTemplate is null)
        {
            return Result<Exception>.Failure(
                new KeyNotFoundException($"StudyTemplate with id {studyTemplateId} not found."));
        }

        var removeResult = studyTemplate.RemoveStudyTemplateObservation(new StudyTemplateObservationId(parameterId));

        if (removeResult.IsFailure)
        {
            return Result<Exception>.Failure(removeResult.Error!);
        }

        try
        {
            await repository.SaveChangesAsync(cancellationToken);
            return Result<Exception>.Success();
        }
        catch (Exception ex)
        {
            return Result<Exception>.Failure(new Exception($"Failed to remove parameter: {ex.Message}", ex));
        }
    }
}
