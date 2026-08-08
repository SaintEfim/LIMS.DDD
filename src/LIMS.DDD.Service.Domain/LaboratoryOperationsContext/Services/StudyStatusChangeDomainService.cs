using LIMS.DDD.Service.Domain.LaboratoryOperationsContext.SampleAggregate;
using LIMS.DDD.Service.Domain.LaboratoryOperationsContext.SampleAggregate.ValueObjects;
using LIMS.DDD.Service.Domain.LaboratoryOperationsContext.StudyAggregate;
using LIMS.DDD.Service.Domain.LaboratoryOperationsContext.StudyAggregate.ValueObjects;
using LIMS.DDD.Service.Domain.SeedWork.Result;

namespace LIMS.DDD.Service.Domain.LaboratoryOperationsContext.Services;

public sealed class StudyStatusChangeDomainService
{
    public Result<None, Exception> ValidateAndChangeStatus(
        Study study,
        StudyStatus newStatus,
        Sample parentSample)
    {
        if ((newStatus == StudyStatus.Completed || newStatus == StudyStatus.Approved) &&
            parentSample.SampleStatus == SampleStatus.Canceled)
        {
            return Result<None, Exception>.Failure(
                new InvalidOperationException("Cannot complete or approve a study for a canceled sample."));
        }

        return study.ChangeStatus(newStatus);
    }
}
