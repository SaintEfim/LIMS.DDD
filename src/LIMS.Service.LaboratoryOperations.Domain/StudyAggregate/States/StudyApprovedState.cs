using LIMS.Service.LaboratoryOperations.Domain.SeedWork;
using LIMS.Service.LaboratoryOperations.Domain.SeedWork.Result;

namespace LIMS.Service.LaboratoryOperations.Domain.StudyAggregate.States;

public sealed class StudyApprovedState : IState<Study>
{
    public string Name => "Approved";
    public bool CanEdit => false;

    public Result<None, Exception> CanTransitionTo(
        IState<Study> newState,
        Study study)
    {
        return Result<None, Exception>.Failure(new InvalidOperationException("Approved studies cannot change status."));
    }
}
