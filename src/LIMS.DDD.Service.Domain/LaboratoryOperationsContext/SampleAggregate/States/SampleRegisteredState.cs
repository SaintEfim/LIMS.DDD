using LIMS.DDD.Service.Domain.SeedWork;
using LIMS.DDD.Service.Domain.SeedWork.Result;

namespace LIMS.DDD.Service.Domain.LaboratoryOperationsContext.SampleAggregate.States;

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
            SampleInProgressState or SampleRegisteredState => Result<None, Exception>.Success(new None()),
            SampleCanceledState => Result<None, Exception>.Success(new None()),
            _ => Result<None, Exception>.Failure(new InvalidOperationException("Invalid transition from Registered"))
        };
    }
}
