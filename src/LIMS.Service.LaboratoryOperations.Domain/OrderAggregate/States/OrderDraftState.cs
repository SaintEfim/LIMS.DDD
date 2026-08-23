using Domain.SeedWork;
using Domain.SeedWork.Errors;
using Domain.SeedWork.Result;

namespace LIMS.Service.LaboratoryOperations.Domain.OrderAggregate.States;

public sealed class OrderDraftState : IState<Order>
{
    public string Name => "Draft";
    public bool CanEdit => true;

    public Result<None, InvalidStatusTransitionError> CanTransitionTo(
        IState<Order> newState,
        Order template)
    {
        return newState switch
        {
            OrderInProgressState or OrderCanceledState or OrderDraftState => new None(),
            _ => new InvalidStatusTransitionError(nameof(Order), Name, newState.Name)
        };
    }
}
