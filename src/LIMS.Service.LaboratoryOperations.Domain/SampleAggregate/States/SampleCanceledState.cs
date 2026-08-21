using Domain.SeedWork;
using Domain.SeedWork.Result;

namespace LIMS.Service.LaboratoryOperations.Domain.SampleAggregate.States;

public sealed class SampleCanceledState : IState<Sample>
{
    public string Name => "Canceled";

    public bool CanEdit => false;

    public Result<None, Exception> CanTransitionTo(
        IState<Sample> newState,
        Sample sample)
    {
        return newState switch
        {
            SampleCanceledState => new None(),
            _ => new InvalidOperationException("Cannot transition from Canceled state. A canceled sample is final.")
        };
    }
}
