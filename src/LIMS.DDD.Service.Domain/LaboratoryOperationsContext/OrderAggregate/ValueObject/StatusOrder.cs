using LIMS.DDD.Service.Domain.LaboratoryOperationsContext.OrderAggregate.State;
using LIMS.DDD.Service.Domain.SeedWork;
using LIMS.DDD.Service.Domain.SeedWork.ValueObjects;

namespace LIMS.DDD.Service.Domain.LaboratoryOperationsContext.OrderAggregate.ValueObject;

public sealed record StatusOrder : StatusBase<IState<Order>, Order>
{
    public static StatusOrder Draft { get; } = new(new DraftState());
    public static StatusOrder Completed { get; } = new(new CompletedState());

    private static readonly Dictionary<string, StatusOrder> Registry = new(StringComparer.OrdinalIgnoreCase)
    {
        ["Draft"] = Draft,
        ["Completed"] = Completed
    };

    private StatusOrder(
        IState<Order> state)
        : base(state)
    {
    }

    public static bool TryParse(
        string name,
        out StatusOrder? status)
    {
        return Registry.TryGetValue(name, out status);
    }

    public static StatusOrder ConvertStatus(
        string value)
    {
        return TryParse(value, out var status)
            ? status!
            : throw new InvalidOperationException($"Unknown status '{value}'");
    }
}
