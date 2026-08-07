using LIMS.DDD.Service.Domain.LaboratoryOperationsContext.SampleAggregate.States;
using LIMS.DDD.Service.Domain.SeedWork;
using LIMS.DDD.Service.Domain.SeedWork.ValueObjects;

namespace LIMS.DDD.Service.Domain.LaboratoryOperationsContext.SampleAggregate.ValueObjects;

public sealed record SampleStatus : StatusBase<IState<Sample>, Sample>
{
    private SampleStatus(
        IState<Sample> state)
        : base(state)
    {
    }

    public static SampleStatus Registered { get; } = new(new SampleRegisteredState());

    public static SampleStatus InProgress { get; } = new(new SampleInProgressState());

    public static SampleStatus Completed { get; } = new(new SampleCompletedState());

    public static SampleStatus Canceled { get; } = new(new SampleCanceledState());

    private static readonly Dictionary<string, SampleStatus> Registry = new(StringComparer.OrdinalIgnoreCase)
    {
        ["Registered"] = Registered,
        ["InProgress"] = InProgress,
        ["Completed"] = Completed,
        ["Canceled"] = Canceled
    };

    public static bool TryParse(
        string? name,
        out SampleStatus? status)
    {
        status = null;
        return !string.IsNullOrWhiteSpace(name) && Registry.TryGetValue(name, out status);
    }

    public static SampleStatus ConvertStatus(
        string? value)
    {
        return TryParse(value, out var status)
            ? status!
            : throw new InvalidOperationException($"Unknown status '{value ?? "null"}'");
    }
}
