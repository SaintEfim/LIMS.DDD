using LIMS.DDD.Service.Domain.SeedWork;
using LIMS.DDD.Service.Domain.SeedWork.Result;

namespace LIMS.DDD.Service.Domain.LaboratoryOperationsContext.SampleAggregate.States;

public sealed class SampleInProgressState : IState<Sample>
{
    public string Name => "InProgress";
    public bool CanEdit => false;

    public Result<UnitEmpty, Exception> CanTransitionTo(
        IState<Sample> newState,
        Sample sample)
    {
        return newState switch
        {
            SampleCompletedState => Result<UnitEmpty, Exception>.Success(new UnitEmpty()),
            SampleCanceledState => Result<UnitEmpty, Exception>.Success(new UnitEmpty()),
            _ => Result<UnitEmpty, Exception>.Failure(new InvalidOperationException("Invalid transition from InWork"))
        };
    }
}
