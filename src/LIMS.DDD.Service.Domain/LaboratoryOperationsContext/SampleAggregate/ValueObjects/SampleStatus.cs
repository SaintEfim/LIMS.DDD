using LIMS.DDD.Service.Domain.LaboratoryOperationsContext.SampleAggregate.State;
using LIMS.DDD.Service.Domain.SeedWork;
using LIMS.DDD.Service.Domain.SeedWork.ValueObjects;

namespace LIMS.DDD.Service.Domain.LaboratoryOperationsContext.SampleAggregate.ValueObjects;

public sealed record SampleStatus : StatusBase<IState<Sample>, Sample>
{
    public static SampleStatus Registered { get; } = new(new SampleRegisteredState());
    public static SampleStatus InWork { get; } = new(new SampleInWorkState());
    public static SampleStatus Completed { get; } = new(new SampleCompletedState());

    private static readonly Dictionary<string, SampleStatus> Registry = new(StringComparer.OrdinalIgnoreCase)
    {
        ["Registered"] = Registered,
        ["InWork"] = InWork,
        ["Completed"] = Completed
    };

    private SampleStatus(
        IState<Sample> state)
        : base(state)
    {
    }

    public static bool TryParse(
        string name,
        out SampleStatus? status)
    {
        return Registry.TryGetValue(name, out status);
    }

    public static SampleStatus ConvertStatus(
        string value)
    {
        return TryParse(value, out var status)
            ? status!
            : throw new InvalidOperationException($"Unknown status '{value}'");
    }
}
