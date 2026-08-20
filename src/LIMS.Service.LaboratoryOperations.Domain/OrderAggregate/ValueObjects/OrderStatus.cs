using Domain.SeedWork.SeedWork;
using Domain.SeedWork.SeedWork.ValueObjects;
using LIMS.Service.LaboratoryOperations.Domain.OrderAggregate.States;

namespace LIMS.Service.LaboratoryOperations.Domain.OrderAggregate.ValueObjects;

public sealed record OrderStatus : StatusBase<IState<Order>, Order>
{
    public static OrderStatus Draft { get; } = new(new OrderDraftState());

    public static OrderStatus InProgress { get; } = new(new OrderInProgressState());

    public static OrderStatus Completed { get; } = new(new OrderCompletedState());

    public static OrderStatus Canceled { get; } = new(new OrderCanceledState());

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

    public static bool TryParse(
        string? name,
        out OrderStatus? status)
    {
        status = null;
        return name is not null && Registry.TryGetValue(name, out status);
    }

    public static OrderStatus ConvertStatus(
        string? value)
    {
        return TryParse(value, out var status) && status is not null
            ? status
            : throw new InvalidOperationException($"Unknown status '{value}'");
    }
}
