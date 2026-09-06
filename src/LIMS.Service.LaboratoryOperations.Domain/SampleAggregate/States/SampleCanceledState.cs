using Library.Domain.SeedWork;
using Library.Domain.SeedWork.Errors;
using Library.Domain.SeedWork.Result;

namespace LIMS.Service.LaboratoryOperations.Domain.SampleAggregate.States;

public sealed class SampleCanceledState : IState<Sample>
{
    public string Name => "Canceled";

    public bool CanEdit => false;

    public Result<None, InvalidStatusTransitionError> CanTransitionTo(
        IState<Sample> newState,
        Sample sample)
    {
        return newState switch
        {
            SampleCanceledState => new None(),
            _ => new InvalidStatusTransitionError(nameof(Sample), Name, newState.Name,
                "Cannot transition from Canceled state. A canceled sample is final.")
        };
    }
}
