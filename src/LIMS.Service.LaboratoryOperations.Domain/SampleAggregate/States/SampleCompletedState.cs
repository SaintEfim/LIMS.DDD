using Domain.SeedWork.SeedWork;
using Domain.SeedWork.SeedWork.Result;

namespace LIMS.Service.LaboratoryOperations.Domain.SampleAggregate.States;

public sealed class SampleCompletedState : IState<Sample>
{
    public string Name => "Completed";
    public bool CanEdit => false;

    public Result<None, Exception> CanTransitionTo(
        IState<Sample> newState,
        Sample sample)
    {
        return Result<None, Exception>.Failure(
            new InvalidOperationException("Completed samples cannot change status."));
    }
}
