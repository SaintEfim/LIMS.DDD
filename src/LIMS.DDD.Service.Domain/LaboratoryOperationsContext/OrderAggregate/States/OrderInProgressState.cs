using LIMS.DDD.Service.Domain.SeedWork;
using LIMS.DDD.Service.Domain.SeedWork.Result;

namespace LIMS.DDD.Service.Domain.LaboratoryOperationsContext.OrderAggregate.States;

public sealed class OrderInProgressState : IState<Order>
{
    public string Name => "InProgress";
    public bool CanEdit => false;

    public Result<UnitEmpty, Exception> CanTransitionTo(
        IState<Order> newState,
        Order template)
    {
        return newState switch
        {
            OrderCompletedState => Result<UnitEmpty, Exception>.Success(new UnitEmpty()),
            OrderCanceledState => Result<UnitEmpty, Exception>.Success(new UnitEmpty()),
            _ => Result<UnitEmpty, Exception>.Failure(new InvalidOperationException("Invalid transition from InWork"))
        };
    }
}
