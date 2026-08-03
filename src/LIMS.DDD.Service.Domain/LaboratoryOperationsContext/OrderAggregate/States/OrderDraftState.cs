using LIMS.DDD.Service.Domain.SeedWork;
using LIMS.DDD.Service.Domain.SeedWork.Result;

namespace LIMS.DDD.Service.Domain.LaboratoryOperationsContext.OrderAggregate.States;

public sealed class OrderDraftState : IState<Order>
{
    public string Name => "Draft";
    public bool CanEdit => true;

    public Result<Exception> CanTransitionTo(
        IState<Order> newState,
        Order template)
    {
        return newState switch
        {
            OrderInProgressState or OrderCanceledState or OrderDraftState => Result<Exception>.Success(),
            _ => Result<Exception>.Failure(new InvalidOperationException("Invalid transition from Draft"))
        };
    }
}
