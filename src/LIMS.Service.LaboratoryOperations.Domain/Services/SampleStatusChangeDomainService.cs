using Domain.SeedWork.Errors;
using Domain.SeedWork.Result;
using LIMS.Service.LaboratoryOperations.Domain.SampleAggregate;
using LIMS.Service.LaboratoryOperations.Domain.SampleAggregate.ValueObjects;
using LIMS.Service.LaboratoryOperations.Domain.StudyAggregate;
using LIMS.Service.LaboratoryOperations.Domain.StudyAggregate.ValueObjects;

namespace LIMS.Service.LaboratoryOperations.Domain.Services;

public sealed class SampleStatusChangeDomainService
{
    public Result<None, DomainError> ValidateAndChangeStatus(
        Sample sample,
        SampleStatus newStatus,
        IReadOnlyCollection<Study> associatedStudies)
    {
        if (newStatus == SampleStatus.Completed)
        {
            var hasActiveStudies = associatedStudies.Any(s => s.Status == StudyStatus.InProgress);
            if (hasActiveStudies)
            {
                return new InvalidStatusTransitionError(nameof(Sample), sample.SampleStatus.Name, newStatus.Name,
                    "Cannot complete the sample because there are studies in 'InProgress' status. " +
                    "Please complete all studies first.");
            }
        }

        if (newStatus == SampleStatus.Canceled)
        {
            var hasCompletedStudies = associatedStudies.Any(s =>
                s.Status == StudyStatus.Completed || s.Status == StudyStatus.Approved);
            if (hasCompletedStudies)
            {
                return new InvalidStatusTransitionError(nameof(Sample), sample.SampleStatus.Name, newStatus.Name,
                    "Cannot cancel the sample because there are completed or approved studies. " +
                    "Please cancel the studies first.");
            }
        }

        var changeResult = sample.ChangeStatus(newStatus);
        if (changeResult.IsFailure)
        {
            return changeResult.GetError();
        }

        return new None();
    }
}
