using Domain.SeedWork;
using Domain.SeedWork.Errors;
using Domain.SeedWork.Result;

namespace LIMS.Service.LaboratoryOperations.Domain.StudyAggregate.States;

public sealed class StudyCompletedState : IState<Study>
{
    public string Name => "Completed";
    public bool CanEdit => false;

    public Result<None, InvalidStatusTransitionError> CanTransitionTo(
        IState<Study> newState,
        Study study)
    {
        return newState switch
        {
            StudyCompletedState => new None(),
            StudyApprovedState or StudyInProgressState => new None(),
            _ => new InvalidStatusTransitionError(nameof(Study), Name, newState.Name)
        };
    }
}
