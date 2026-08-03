using LIMS.DDD.Service.Domain.SeedWork;
using LIMS.DDD.Service.Domain.SeedWork.Result;

namespace LIMS.DDD.Service.Domain.LaboratoryOperationsContext.OrderAggregate.States;

public sealed class OrderCanceledState : IState<Order>
{
    public string Name => "Canceled";
    public bool CanEdit => false;

    public Result<Exception> CanTransitionTo(
        IState<Order> newState,
        Order template)
    {
        return Result<Exception>.Failure(new InvalidOperationException("Canceled orders cannot change status."));
    }
}
