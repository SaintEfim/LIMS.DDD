using Domain.SeedWork;
using Domain.SeedWork.Errors;
using Domain.SeedWork.Result;

namespace LIMS.Service.LaboratoryOperations.Domain.StudyAggregate.States;

public sealed class StudyCanceledState : IState<Study>
{
    public string Name => "Canceled";

    public bool CanEdit => false;

    public Result<None, InvalidStatusTransitionError> CanTransitionTo(
        IState<Study> newState,
        Study study)
    {
        return newState switch
        {
            StudyCanceledState => new None(),
            _ => new InvalidStatusTransitionError(nameof(Study), Name, newState.Name,
                "Canceled studies cannot change status.")
        };
    }
}
