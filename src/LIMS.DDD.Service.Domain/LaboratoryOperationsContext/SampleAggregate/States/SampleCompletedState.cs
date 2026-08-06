using LIMS.DDD.Service.Domain.SeedWork;
using LIMS.DDD.Service.Domain.SeedWork.Result;

namespace LIMS.DDD.Service.Domain.LaboratoryOperationsContext.SampleAggregate.States;

public sealed class SampleCompletedState : IState<Sample>
{
    public string Name => "Completed";
    public bool CanEdit => false;

    public Result<UnitEmpty, Exception> CanTransitionTo(
        IState<Sample> newState,
        Sample sample)
    {
        return Result<UnitEmpty, Exception>.Failure(
            new InvalidOperationException("Completed samples cannot change status."));
    }
}
