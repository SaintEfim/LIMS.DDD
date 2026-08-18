using LIMS.Service.LaboratoryOperations.Domain.SeedWork;
using LIMS.Service.LaboratoryOperations.Domain.SeedWork.Result;

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
            SampleCanceledState => Result<None, Exception>.Success(new None()),
            _ => Result<None, Exception>.Failure(
                new InvalidOperationException("Cannot transition from Canceled state. A canceled sample is final."))
        };
    }
}
