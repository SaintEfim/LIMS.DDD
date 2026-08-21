using Domain.SeedWork.SeedWork;
using Domain.SeedWork.SeedWork.Result;

namespace LIMS.Service.LaboratoryOperations.Domain.OrderAggregate.States;

public sealed class OrderInProgressState : IState<Order>
{
    public string Name => "InProgress";
    public bool CanEdit => false;

    public Result<None, Exception> CanTransitionTo(
        IState<Order> newState,
        Order template)
    {
        return newState switch
        {
            OrderCompletedState => new None(),
            OrderCanceledState => new None(),
            _ => new InvalidOperationException("Invalid transition from InWork")
        };
    }
}
