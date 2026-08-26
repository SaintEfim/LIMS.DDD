using Domain.SeedWork;
using Domain.SeedWork.Errors;
using Domain.SeedWork.Result;

namespace LIMS.Service.LaboratoryOperations.Domain.OrderAggregate.States;

public sealed class OrderCanceledState : IState<Order>
{
    public string Name => "Canceled";
    public bool CanEdit => false;

    public Result<None, InvalidStatusTransitionError> CanTransitionTo(
        IState<Order> newState,
        Order order)
    {
        return newState switch
        {
            OrderCanceledState => Result<None, InvalidStatusTransitionError>.Success(), // или new None()
            _ => Result<None, InvalidStatusTransitionError>.Failure(new InvalidStatusTransitionError(nameof(Order),
                Name, newState.Name, "Canceled orders cannot change status."))
        };
    }
}
