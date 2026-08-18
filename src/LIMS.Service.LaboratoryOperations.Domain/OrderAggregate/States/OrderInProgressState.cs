using LIMS.Service.LaboratoryOperations.Domain.SeedWork;
using LIMS.Service.LaboratoryOperations.Domain.SeedWork.Result;

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
            OrderCompletedState => Result<None, Exception>.Success(new None()),
            OrderCanceledState => Result<None, Exception>.Success(new None()),
            _ => Result<None, Exception>.Failure(new InvalidOperationException("Invalid transition from InWork"))
        };
    }
}
