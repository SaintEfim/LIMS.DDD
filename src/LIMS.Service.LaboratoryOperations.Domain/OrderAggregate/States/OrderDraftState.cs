using LIMS.Service.LaboratoryOperations.Domain.SeedWork;
using LIMS.Service.LaboratoryOperations.Domain.SeedWork.Result;

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
            OrderInProgressState or OrderCanceledState or OrderDraftState =>
                Result<None, Exception>.Success(new None()),
            _ => Result<None, Exception>.Failure(new InvalidOperationException("Invalid transition from Draft"))
        };
    }
}
