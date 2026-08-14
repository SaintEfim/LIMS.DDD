using LIMS.DDD.Service.Domain.LaboratoryOperationsContext.StudyAggregate.States;
using LIMS.DDD.Service.Domain.SeedWork;
using LIMS.DDD.Service.Domain.SeedWork.ValueObjects;

namespace LIMS.DDD.Service.Domain.LaboratoryOperationsContext.StudyAggregate.ValueObjects;

public sealed record StudyStatus : StatusBase<IState<Study>, Study>
{
    private static readonly Dictionary<string, StudyStatus> Registry = new(StringComparer.OrdinalIgnoreCase)
    {
        ["InProgress"] = InProgress,
        ["Completed"] = Completed,
        ["Approved"] = Approved,
        ["Canceled"] = Canceled
    };

    private StudyStatus(
        IState<Study> state)
        : base(state)
    {
    }

    public static StudyStatus InProgress { get; } = new(new StudyInProgressState());

    public static StudyStatus Completed { get; } = new(new StudyCompletedState());

    public static StudyStatus Approved { get; } = new(new StudyApprovedState());

    public static StudyStatus Canceled { get; } = new(new StudyCanceledState());

    public static bool TryParse(
        string? name,
        out StudyStatus? status)
    {
        status = null;
        return name is not null && Registry.TryGetValue(name, out status);
    }

    public static StudyStatus ConvertStatus(
        string? value)
    {
        return TryParse(value, out var status) && status is not null
            ? status
            : throw new InvalidOperationException($"Unknown status '{value}'");
    }
}
