using LIMS.Service.LaboratoryOperations.Domain.SeedWork;
using LIMS.Service.LaboratoryOperations.Domain.SeedWork.Result;

namespace LIMS.Service.LaboratoryOperations.Domain.OrderAggregate.States;

public sealed class OrderCanceledState : IState<Order>
{
    public string Name => "Canceled";
    public bool CanEdit => false;

    public Result<None, Exception> CanTransitionTo(
        IState<Order> newState,
        Order template)
    {
        return Result<None, Exception>.Failure(new InvalidOperationException("Canceled orders cannot change status."));
    }
}
