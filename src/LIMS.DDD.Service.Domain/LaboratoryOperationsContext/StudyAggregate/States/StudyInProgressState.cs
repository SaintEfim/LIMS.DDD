using LIMS.DDD.Service.Domain.SeedWork;
using LIMS.DDD.Service.Domain.SeedWork.Result;

namespace LIMS.DDD.Service.Domain.LaboratoryOperationsContext.StudyAggregate.States;

public sealed class StudyInProgressState : IState<Study>
{
    public string Name => "InProgress";
    public bool CanEdit => true;

    public Result<UnitEmpty, Exception> CanTransitionTo(
        IState<Study> newState,
        Study study)
    {
        return newState switch
        {
            StudyInProgressState or StudyCompletedState => Result<UnitEmpty, Exception>.Success(new UnitEmpty()),
            _ => Result<UnitEmpty, Exception>.Failure(new InvalidOperationException("Invalid transition from InWork"))
        };
    }
}
