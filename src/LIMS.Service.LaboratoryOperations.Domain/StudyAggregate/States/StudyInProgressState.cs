using Domain.SeedWork;
using Domain.SeedWork.Errors;
using Domain.SeedWork.Result;

namespace LIMS.Service.LaboratoryOperations.Domain.StudyAggregate.States;

public sealed class StudyInProgressState : IState<Study>
{
    public string Name => "InProgress";
    public bool CanEdit => true;

    public Result<None, InvalidStatusTransitionError> CanTransitionTo(
        IState<Study> newState,
        Study study)
    {
        return newState switch
        {
            StudyInProgressState or StudyCompletedState or StudyCanceledState => new None(),
            _ => new InvalidStatusTransitionError(nameof(Study), Name, newState.Name)
        };
    }
}
