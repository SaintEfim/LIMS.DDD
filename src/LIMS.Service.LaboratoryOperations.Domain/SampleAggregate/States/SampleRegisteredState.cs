using Domain.SeedWork.SeedWork;
using Domain.SeedWork.SeedWork.Result;

namespace LIMS.Service.LaboratoryOperations.Domain.SampleAggregate.States;

public sealed class SampleRegisteredState : IState<Sample>
{
    public string Name => "Registered";
    public bool CanEdit => true;

    public Result<None, Exception> CanTransitionTo(
        IState<Sample> newState,
        Sample sample)
    {
        return newState switch
        {
            SampleInProgressState or SampleRegisteredState => new None(),
            SampleCanceledState => new None(),
            _ => new InvalidOperationException("Invalid transition from Registered")
        };
    }
}
