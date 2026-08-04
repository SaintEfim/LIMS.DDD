using LIMS.DDD.Service.Domain.SeedWork;
using LIMS.DDD.Service.Domain.SeedWork.Result;

namespace LIMS.DDD.Service.Domain.LaboratoryOperationsContext.SampleAggregate.States;

public sealed class SampleInProgressState : IState<Sample>
{
    public string Name => "InProgress";
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
