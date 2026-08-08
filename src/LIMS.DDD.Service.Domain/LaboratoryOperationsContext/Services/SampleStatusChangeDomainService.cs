using LIMS.DDD.Service.Domain.LaboratoryOperationsContext.SampleAggregate;
using LIMS.DDD.Service.Domain.LaboratoryOperationsContext.SampleAggregate.ValueObjects;
using LIMS.DDD.Service.Domain.LaboratoryOperationsContext.StudyAggregate;
using LIMS.DDD.Service.Domain.LaboratoryOperationsContext.StudyAggregate.ValueObjects;
using LIMS.DDD.Service.Domain.SeedWork.Result;

namespace LIMS.DDD.Service.Domain.LaboratoryOperationsContext.Services;

public sealed class SampleStatusChangeDomainService
{
    public Result<None, Exception> ValidateAndChangeStatus(
        Sample sample,
        SampleStatus newStatus,
        IReadOnlyCollection<Study> associatedStudies)
    {
        if (newStatus == SampleStatus.Completed)
        {
            var hasActiveStudies = associatedStudies.Any(s => s.Status == StudyStatus.InProgress);
            if (hasActiveStudies)
            {
                return Result<None, Exception>.Failure(new InvalidOperationException(
                    "Cannot complete the sample because there are studies in 'InProgress' status. " +
                    "Please complete all studies first."));
            }
        }

        if (newStatus == SampleStatus.Canceled)
        {
            var hasCompletedStudies = associatedStudies.Any(s =>
                s.Status == StudyStatus.Completed || s.Status == StudyStatus.Approved);
            if (hasCompletedStudies)
            {
                return Result<None, Exception>.Failure(new InvalidOperationException(
                    "Cannot cancel the sample because there are completed or approved studies. " +
                    "Please cancel the studies first."));
            }
        }

        return sample.ChangeStatus(newStatus);
    }
}
