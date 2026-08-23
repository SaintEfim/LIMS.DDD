using Domain.SeedWork.Errors;
using Domain.SeedWork.Result;
using LIMS.Service.LaboratoryOperations.Domain.SampleAggregate;
using LIMS.Service.LaboratoryOperations.Domain.SampleAggregate.ValueObjects;
using LIMS.Service.LaboratoryOperations.Domain.StudyAggregate;
using LIMS.Service.LaboratoryOperations.Domain.StudyAggregate.ValueObjects;

namespace LIMS.Service.LaboratoryOperations.Domain.Services;

public sealed class StudyStatusChangeDomainService
{
    public Result<None, DomainError> ValidateAndChangeStatus(
        Study study,
        StudyStatus newStatus,
        Sample parentSample)
    {
        if ((newStatus == StudyStatus.Completed || newStatus == StudyStatus.Approved) &&
            parentSample.SampleStatus == SampleStatus.Canceled)
        {
            return new InvalidStatusTransitionError(nameof(Study), study.Status.Name, newStatus.Name,
                "Cannot complete or approve a study for a canceled sample.");
        }

        var changeResult = study.ChangeStatus(newStatus);
        if (changeResult.IsFailure)
        {
            return changeResult.GetError();
        }

        return new None();
    }
}
