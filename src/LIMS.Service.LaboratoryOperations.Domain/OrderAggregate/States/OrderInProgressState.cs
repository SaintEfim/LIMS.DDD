using Domain.SeedWork;
using Domain.SeedWork.Errors;
using Domain.SeedWork.Result;

namespace LIMS.Service.LaboratoryOperations.Domain.OrderAggregate.States;

public sealed class OrderInProgressState : IState<Order>
{
    public string Name => "InProgress";
    public bool CanEdit => false;

    public Result<None, InvalidStatusTransitionError> CanTransitionTo(
        IState<Order> newState,
        Order template)
    {
        return newState switch
        {
            OrderCompletedState or OrderCanceledState or OrderInProgressState => new None(),
            _ => new InvalidStatusTransitionError(nameof(Order), Name, newState.Name)
        };
    }
}
