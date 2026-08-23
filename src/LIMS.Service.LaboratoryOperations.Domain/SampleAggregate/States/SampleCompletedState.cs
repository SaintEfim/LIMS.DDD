using Domain.SeedWork;
using Domain.SeedWork.Errors;
using Domain.SeedWork.Result;

namespace LIMS.Service.LaboratoryOperations.Domain.SampleAggregate.States;

public sealed class SampleCompletedState : IState<Sample>
{
    public string Name => "Completed";
    public bool CanEdit => false;

    public Result<None, InvalidStatusTransitionError> CanTransitionTo(
        IState<Sample> newState,
        Sample sample)
    {
        return newState switch
        {
            SampleCompletedState => new None(),
            _ => new InvalidStatusTransitionError(nameof(Sample), Name, newState.Name,
                "Completed samples cannot change status.")
        };
    }
}
