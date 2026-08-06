using LIMS.DDD.Service.Domain.LaboratoryOperationsContext.OrderAggregate.States;
using LIMS.DDD.Service.Domain.SeedWork;
using LIMS.DDD.Service.Domain.SeedWork.ValueObjects;

namespace LIMS.DDD.Service.Domain.LaboratoryOperationsContext.OrderAggregate.ValueObjects;

public sealed record OrderStatus : StatusBase<IState<Order>, Order>
{
    private static readonly Dictionary<string, OrderStatus> Registry = new(StringComparer.OrdinalIgnoreCase)
    {
        ["Draft"] = Draft,
        ["InProgress"] = InProgress,
        ["Completed"] = Completed,
        ["Canceled"] = Canceled
    };

    private OrderStatus(
        IState<Order> state)
        : base(state)
    {
    }

    public static OrderStatus Draft { get; } = new(new OrderDraftState());

    public static OrderStatus InProgress { get; } = new(new OrderInProgressState());

    public static OrderStatus Completed { get; } = new(new OrderCompletedState());

    public static OrderStatus Canceled { get; } = new(new OrderCanceledState());

    public static bool TryParse(
        string name,
        out OrderStatus status)
    {
        return Registry.TryGetValue(name, out status!);
    }

    public static OrderStatus ConvertStatus(
        string value)
    {
        return TryParse(value, out var status)
            ? status
            : throw new InvalidOperationException($"Unknown status '{value}'");
    }
}
