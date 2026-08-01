using LIMS.DDD.Service.Domain.LaboratoryOperationsContext.OrderAggregate.State;
using LIMS.DDD.Service.Domain.SeedWork;
using LIMS.DDD.Service.Domain.SeedWork.ValueObjects;

namespace LIMS.DDD.Service.Domain.LaboratoryOperationsContext.OrderAggregate.ValueObject;

public sealed record OrderStatus : StatusBase<IState<Order>, Order>
{
    public static OrderStatus Draft { get; } = new(new DraftState());
    public static OrderStatus Completed { get; } = new(new CompletedState());

    private static readonly Dictionary<string, OrderStatus> Registry = new(StringComparer.OrdinalIgnoreCase)
    {
        ["Draft"] = Draft,
        ["Completed"] = Completed
    };

    private OrderStatus(
        IState<Order> state)
        : base(state)
    {
    }

    public static bool TryParse(
        string name,
        out OrderStatus? status)
    {
        return Registry.TryGetValue(name, out status);
    }

    public static OrderStatus ConvertStatus(
        string value)
    {
        return TryParse(value, out var status)
            ? status!
            : throw new InvalidOperationException($"Unknown status '{value}'");
    }
}
