using Domain.SeedWork.SeedWork;
using Domain.SeedWork.SeedWork.Result;

namespace LIMS.Service.LaboratoryOperations.Domain.OrderAggregate.States;

public sealed class OrderCompletedState : IState<Order>
{
    public string Name => "Completed";
    public bool CanEdit => false;

    public Result<None, Exception> CanTransitionTo(
        IState<Order> newState,
        Order template)
    {
        return newState switch
        {
            OrderCompletedState => new None(),
            _ => new InvalidOperationException("Completed orders cannot change status.")
        };
    }
}
