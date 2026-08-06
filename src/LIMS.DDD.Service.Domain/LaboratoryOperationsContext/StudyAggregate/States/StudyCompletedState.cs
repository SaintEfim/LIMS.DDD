using LIMS.DDD.Service.Domain.SeedWork;
using LIMS.DDD.Service.Domain.SeedWork.Result;

namespace LIMS.DDD.Service.Domain.LaboratoryOperationsContext.StudyAggregate.States;

public sealed class StudyCompletedState : IState<Study>
{
    public string Name => "Completed";
    public bool CanEdit => false;

    public Result<UnitEmpty, Exception> CanTransitionTo(
        IState<Study> newState,
        Study study)
    {
        return newState switch
        {
            StudyCompletedState or StudyApprovedState or StudyInProgressState => Result<UnitEmpty, Exception>.Success(
                new UnitEmpty()),
            _ => Result<UnitEmpty, Exception>.Failure(
                new InvalidOperationException("Invalid transition from Completed"))
        };
    }
}
