using Domain.SeedWork.SeedWork;
using Domain.SeedWork.SeedWork.Result;

namespace LIMS.Service.LaboratoryOperations.Domain.OrderAggregate.States;

public sealed class OrderDraftState : IState<Order>
{
    public string Name => "Draft";
    public bool CanEdit => true;

    public Result<None, Exception> CanTransitionTo(
        IState<Order> newState,
        Order template)
    {
        return newState switch
        {
            OrderInProgressState or OrderCanceledState or OrderDraftState => new None(),
            _ => new InvalidOperationException("Invalid transition from Draft")
        };
    }
}
