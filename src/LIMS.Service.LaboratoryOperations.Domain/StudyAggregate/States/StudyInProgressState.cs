using Domain.SeedWork;
using Domain.SeedWork.Result;

namespace LIMS.Service.LaboratoryOperations.Domain.StudyAggregate.States;

public sealed class StudyInProgressState : IState<Study>
{
    public string Name => "InProgress";
    public bool CanEdit => true;

    public Result<None, Exception> CanTransitionTo(
        IState<Study> newState,
        Study study)
    {
        return newState switch
        {
            StudyInProgressState or StudyCompletedState => new None(),
            _ => new InvalidOperationException("Invalid transition from InWork")
        };
    }
}
