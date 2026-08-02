using LIMS.DDD.Service.Domain.SeedWork;
using LIMS.DDD.Service.Domain.SeedWork.Result;

namespace LIMS.DDD.Service.Domain.LaboratoryOperationsContext.SampleAggregate.State;

public sealed class SampleRegisteredState : IState<Sample>
{
    public string Name => "Registered";
    public bool CanEdit => true;

    public Result<Exception> CanTransitionTo(IState<Sample> newState, Sample sample)
    {
        return newState switch
        {
            SampleInWorkState => Result<Exception>.Success(),
            SampleRegisteredState => Result<Exception>.Success(),
            _ => Result<Exception>.Failure(new InvalidOperationException("Invalid transition from Registered"))
        };
    }
}
