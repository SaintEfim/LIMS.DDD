using Domain.SeedWork.SeedWork;
using Domain.SeedWork.SeedWork.Result;

namespace LIMS.Service.LaboratoryOperations.Domain.StudyAggregate.States;

public sealed class StudyCompletedState : IState<Study>
{
    public string Name => "Completed";
    public bool CanEdit => false;

    public Result<None, Exception> CanTransitionTo(
        IState<Study> newState,
        Study study)
    {
        return newState switch
        {
            StudyCompletedState or StudyApprovedState or StudyInProgressState => new None(),
            _ => new InvalidOperationException("Invalid transition from Completed")
        };
    }
}
