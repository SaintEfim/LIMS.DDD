using LIMS.DDD.Service.Domain.SeedWork;
using LIMS.DDD.Service.Domain.SeedWork.Result;

namespace LIMS.DDD.Service.Domain.LaboratoryOperationsContext.OrderAggregate.States;

public sealed class OrderCompletedState : IState<Order>
{
    public string Name => "Completed";
    public bool CanEdit => false;

    public Result<Exception> CanTransitionTo(
        IState<Order> newState,
        Order template)
    {
        return newState switch
        {
            OrderCompletedState => Result<Exception>.Success(),
            _ => Result<Exception>.Failure(new InvalidOperationException("Completed orders cannot change status."))
        };
    }
}
