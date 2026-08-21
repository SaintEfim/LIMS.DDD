using Domain.SeedWork;
using Domain.SeedWork.Result;

namespace LIMS.Service.LaboratoryOperations.Domain.SampleAggregate.States;

public sealed class SampleInProgressState : IState<Sample>
{
    public string Name => "InProgress";
    public bool CanEdit => false;

    public Result<None, Exception> CanTransitionTo(
        IState<Sample> newState,
        Sample sample)
    {
        return newState switch
        {
            SampleCompletedState => new None(),
            SampleCanceledState => new None(),
            _ => new InvalidOperationException("Invalid transition from InWork")
        };
    }
}
