using Application.SeedWork;
using Application.SeedWork.Errors;
using Domain.SeedWork;
using Domain.SeedWork.Result;
using LIMS.Service.LaboratoryOperations.Domain.StudyAggregate;
using LIMS.Service.LaboratoryOperations.Domain.StudyAggregate.Entities;

namespace LIMS.Service.LaboratoryOperations.Application.Studies.MeasuredValues;

public sealed class MeasuredValueCommandsHandler(IUnitOfWork unitOfWork, IStudyRepository studyRepository)
    : ICommandsHandler
{
    public async Task<Result<None, ApplicationError>> UpdateAsync(
        Guid studyId,
        Guid measuredValueId,
        UpdateMeasuredValueCommand command,
        CancellationToken cancellationToken = default)
    {
        var studyResult = await GetStudyForChangeAsync(studyId, cancellationToken);
        if (studyResult.IsFailure)
        {
            return studyResult.CastFailure<None>();
        }

        var updateResult = studyResult.GetValue()
            .UpdateMeasuredValue(new MeasuredValueId(measuredValueId), command.Value);

        if (updateResult.IsFailure)
        {
            return new DomainRuleViolation(updateResult.GetError());
        }

        return await SaveChangesAsync(cancellationToken);
    }

    private async Task<Result<Study, ApplicationError>> GetStudyForChangeAsync(
        Guid studyId,
        CancellationToken ct)
    {
        var study = await studyRepository.GetByIdForChangeAsync(new StudyId(studyId), ct);
        if (study is null)
        {
            return new NotFoundError($"Study with id '{studyId}' not found.");
        }

        return study;
    }

    private async Task<Result<None, ApplicationError>> SaveChangesAsync(
        CancellationToken cancellationToken = default)
    {
        try
        {
            await unitOfWork.SaveChangesAsync(cancellationToken);
            return new None();
        }
        catch (Exception ex)
        {
            return new PersistenceError($"Failed to save changes: {ex.Message}");
        }
    }
}
