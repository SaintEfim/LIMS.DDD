using LIMS.DDD.Service.Domain.SeedWork.Result;

namespace LIMS.DDD.Service.Domain.LaboratoryOperationsContext.OrderAggregate.State;

public sealed class DraftState : IState<Order>
{
    public string Name => "Draft";
    public bool CanEdit => true;

    public Result<Exception> CanTransitionTo(IState<Order> newState, Order template)
    {
        return newState switch
        {
            CompletedState => ValidateForComplition(template),
            DraftState => Result<Exception>.Success(),
            _ => Result<Exception>.Failure(new InvalidOperationException("Invalid transition from Draft"))
        };
    }

    private static Result<Exception> ValidateForComplition(Order template)
    {
        return Result<Exception>.Success();
    }
}
