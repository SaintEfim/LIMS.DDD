using LIMS.Service.LaboratoryOperations.Domain.SeedWork;
using LIMS.Service.LaboratoryOperations.Domain.SeedWork.Result;

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
            SampleCompletedState => Result<None, Exception>.Success(new None()),
            SampleCanceledState => Result<None, Exception>.Success(new None()),
            _ => Result<None, Exception>.Failure(new InvalidOperationException("Invalid transition from InWork"))
        };
    }
}
