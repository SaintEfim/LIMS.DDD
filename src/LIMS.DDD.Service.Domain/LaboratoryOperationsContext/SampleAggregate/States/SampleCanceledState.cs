using LIMS.DDD.Service.Domain.SeedWork;
using LIMS.DDD.Service.Domain.SeedWork.Result;

namespace LIMS.DDD.Service.Domain.LaboratoryOperationsContext.SampleAggregate.States;

public sealed class SampleCanceledState : IState<Sample>
{
    public string Name => "Canceled";

    public bool CanEdit => false;

    public Result<Exception> CanTransitionTo(
        IState<Sample> newState,
        Sample sample)
    {
        return newState switch
        {
            SampleCanceledState => Result<Exception>.Success(),
            _ => Result<Exception>.Failure(
                new InvalidOperationException("Cannot transition from Canceled state. A canceled sample is final."))
        };
    }
}
