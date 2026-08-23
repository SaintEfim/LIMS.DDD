using Domain.SeedWork;
using Domain.SeedWork.Errors;
using Domain.SeedWork.Result;

namespace LIMS.Service.LaboratoryOperations.Domain.StudyAggregate.States;

public sealed class StudyApprovedState : IState<Study>
{
    public string Name => "Approved";
    public bool CanEdit => false;

    public Result<None, InvalidStatusTransitionError> CanTransitionTo(
        IState<Study> newState,
        Study study)
    {
        return newState switch
        {
            StudyApprovedState => new None(),
            _ => new InvalidStatusTransitionError(nameof(Study), Name, newState.Name,
                "Approved studies cannot change status.")
        };
    }
}
