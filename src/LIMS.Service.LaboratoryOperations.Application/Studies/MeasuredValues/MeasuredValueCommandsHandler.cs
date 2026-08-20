using Domain.SeedWork.SeedWork;
using Domain.SeedWork.SeedWork.Result;
using LIMS.Service.LaboratoryOperations.Domain.StudyAggregate;
using LIMS.Service.LaboratoryOperations.Domain.StudyAggregate.Entities;

namespace LIMS.Service.LaboratoryOperations.Application.Studies.MeasuredValues;

public sealed class MeasuredValueCommandsHandler(IUnitOfWork unitOfWork, IStudyRepository studyRepository)
{
    public async Task<Result<None, Exception>> UpdateAsync(
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
            return updateResult;
        }

        return await SaveChangesAsync(cancellationToken);
    }

    private async Task<Result<Study, Exception>> GetStudyForChangeAsync(
        Guid studyId,
        CancellationToken ct)
    {
        var study = await studyRepository.GetByIdForChangeAsync(new StudyId(studyId), ct);
        return study is null ? new KeyNotFoundException($"Study with id {studyId} not found.") : study;
    }

    private async Task<Result<None, Exception>> SaveChangesAsync(
        CancellationToken cancellationToken = default)
    {
        try
        {
            await unitOfWork.SaveChangesAsync(cancellationToken);
            return new None();
        }
        catch (Exception ex)
        {
            return new Exception($"Failed to save changes: {ex.Message}", ex);
        }
    }
}
