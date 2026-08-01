using LIMS.DDD.Service.Domain.SeedWork;
using LIMS.DDD.Service.Domain.SeedWork.Result;

namespace LIMS.DDD.Service.Domain.LaboratoryOperationsContext.OrderAggregate.State;

public sealed class CompletedState : IState<Order>
{
    public string Name => "Completed";
    public bool CanEdit => false;

    public Result<Exception> CanTransitionTo(
        IState<Order> newState,
        Order template)
    {
        return newState switch
        {
            CompletedState => Result<Exception>.Success(),
            _ => Result<Exception>.Failure(new InvalidOperationException("Completed templates cannot change status."))
        };
    }
}
