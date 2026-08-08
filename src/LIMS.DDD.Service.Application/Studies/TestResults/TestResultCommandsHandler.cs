using LIMS.DDD.Service.Domain.LaboratoryOperationsContext.StudyAggregate;
using LIMS.DDD.Service.Domain.LaboratoryOperationsContext.StudyAggregate.Entities;
using LIMS.DDD.Service.Domain.SeedWork;
using LIMS.DDD.Service.Domain.SeedWork.Result;

namespace LIMS.DDD.Service.Application.Studies.TestResults;

public sealed class TestResultCommandsHandler(IUnitOfWork unitOfWork, IStudyRepository studyRepository)
{
    public async Task<Result<None, Exception>> UpdateAsync(
        Guid studyId,
        Guid testResultId,
        UpdateTestResultCommand command,
        CancellationToken cancellationToken = default)
    {
        var studyResult = await GetStudyForChangeAsync(studyId, cancellationToken);
        if (studyResult.IsFailure)
        {
            return studyResult.CastFailure<None>();
        }

        var updateResult = studyResult.GetValue()
            .UpdateTestResult(new TestResultId(testResultId), command.Value, command.IsOutOfSpec);

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
        return study is null
            ? Result<Study, Exception>.Failure(new KeyNotFoundException($"Study with id {studyId} not found."))
            : Result<Study, Exception>.Success(study);
    }

    private async Task<Result<None, Exception>> SaveChangesAsync(
        CancellationToken cancellationToken)
    {
        try
        {
            await unitOfWork.SaveChangesAsync(cancellationToken);
            return Result<None, Exception>.Success();
        }
        catch (Exception ex)
        {
            return Result<None, Exception>.Failure(new Exception($"Failed to save changes: {ex.Message}", ex));
        }
    }
}
