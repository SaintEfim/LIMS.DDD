using Domain.SeedWork;
using Domain.SeedWork.Errors;
using Domain.SeedWork.Result;

namespace LIMS.Service.LaboratoryOperations.Domain.SampleAggregate.States;

public sealed class SampleInProgressState : IState<Sample>
{
    public string Name => "InProgress";
    public bool CanEdit => false;

    public Result<None, InvalidStatusTransitionError> CanTransitionTo(
        IState<Sample> newState,
        Sample sample)
    {
        return newState switch
        {
            SampleCompletedState or SampleCanceledState or SampleInProgressState => new None(),
            _ => new InvalidStatusTransitionError(nameof(Sample), Name, newState.Name)
        };
    }
}
