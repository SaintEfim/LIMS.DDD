using Domain.SeedWork;
using Domain.SeedWork.Errors;
using Domain.SeedWork.Result;

namespace LIMS.Service.LaboratoryOperations.Domain.SampleAggregate.States;

public sealed class SampleRegisteredState : IState<Sample>
{
    public string Name => "Registered";
    public bool CanEdit => true;

    public Result<None, InvalidStatusTransitionError> CanTransitionTo(
        IState<Sample> newState,
        Sample sample)
    {
        return newState switch
        {
            SampleInProgressState or SampleRegisteredState or SampleCanceledState => new None(),
            _ => new InvalidStatusTransitionError(nameof(Sample), Name, newState.Name)
        };
    }
}
