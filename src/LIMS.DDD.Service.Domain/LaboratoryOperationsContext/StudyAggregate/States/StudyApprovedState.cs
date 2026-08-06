using LIMS.DDD.Service.Domain.SeedWork;
using LIMS.DDD.Service.Domain.SeedWork.Result;

namespace LIMS.DDD.Service.Domain.LaboratoryOperationsContext.StudyAggregate.States;

public sealed class StudyApprovedState : IState<Study>
{
    public string Name => "Approved";
    public bool CanEdit => false;

    public Result<UnitEmpty, Exception> CanTransitionTo(
        IState<Study> newState,
        Study study)
    {
        return Result<UnitEmpty, Exception>.Failure(
            new InvalidOperationException("Approved studies cannot change status."));
    }
}
