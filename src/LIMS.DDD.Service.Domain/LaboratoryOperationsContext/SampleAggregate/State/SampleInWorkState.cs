using LIMS.DDD.Service.Domain.SeedWork;
using LIMS.DDD.Service.Domain.SeedWork.Result;

namespace LIMS.DDD.Service.Domain.LaboratoryOperationsContext.SampleAggregate.State;

public sealed class SampleInWorkState : IState<Sample>
{
    public string Name => "InWork";
    public bool CanEdit => false;

    public Result<Exception> CanTransitionTo(IState<Sample> newState, Sample sample)
    {
        return newState switch
        {
            SampleCompletedState => Result<Exception>.Success(),
            _ => Result<Exception>.Failure(new InvalidOperationException("Invalid transition from InWork"))
        };
    }
}
