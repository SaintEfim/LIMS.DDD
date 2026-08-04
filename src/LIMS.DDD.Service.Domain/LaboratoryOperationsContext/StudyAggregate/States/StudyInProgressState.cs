using LIMS.DDD.Service.Domain.SeedWork;
using LIMS.DDD.Service.Domain.SeedWork.Result;

namespace LIMS.DDD.Service.Domain.LaboratoryOperationsContext.StudyAggregate.States;

public sealed class StudyInProgressState : IState<Study>
{
    public string Name => "InProgress";
    public bool CanEdit => true;

    public Result<Exception> CanTransitionTo(IState<Study> newState, Study study)
    {
        return newState switch
        {
            StudyInProgressState or StudyCompletedState => Result<Exception>.Success(),
            _ => Result<Exception>.Failure(new InvalidOperationException("Invalid transition from InWork"))
        };
    }
}
