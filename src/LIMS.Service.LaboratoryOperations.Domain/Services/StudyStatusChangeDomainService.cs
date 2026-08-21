using Domain.SeedWork.SeedWork.Result;
using LIMS.Service.LaboratoryOperations.Domain.SampleAggregate;
using LIMS.Service.LaboratoryOperations.Domain.SampleAggregate.ValueObjects;
using LIMS.Service.LaboratoryOperations.Domain.StudyAggregate;
using LIMS.Service.LaboratoryOperations.Domain.StudyAggregate.ValueObjects;

namespace LIMS.Service.LaboratoryOperations.Domain.Services;

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
            return new InvalidOperationException("Cannot complete or approve a study for a canceled sample.");
        }

        return study.ChangeStatus(newStatus);
    }
}
