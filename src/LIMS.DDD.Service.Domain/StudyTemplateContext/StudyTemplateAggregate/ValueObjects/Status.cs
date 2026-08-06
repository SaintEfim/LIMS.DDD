using LIMS.DDD.Service.Domain.SeedWork;
using LIMS.DDD.Service.Domain.SeedWork.ValueObjects;
using LIMS.DDD.Service.Domain.StudyTemplateContext.StudyTemplateAggregate.States;

namespace LIMS.DDD.Service.Domain.StudyTemplateContext.StudyTemplateAggregate.ValueObjects;

public sealed record Status : StatusBase<IState<StudyTemplate>, StudyTemplate>
{
    private static readonly Dictionary<string, Status> Registry =
        new(StringComparer.OrdinalIgnoreCase)
        {
            ["Draft"] = Draft,
            ["Active"] = Active,
            ["Archived"] = Archived
        };

    private Status(
        IState<StudyTemplate> state)
        : base(state)
    {
    }

    public static Status Draft { get; } = new(new DraftState());
    public static Status Active { get; } = new(new ActiveState());
    public static Status Archived { get; } = new(new ArchivedState());

    public static bool TryParse(
        string name,
        out Status status)
    {
        return Registry.TryGetValue(name, out status!);
    }

    public static Status ConvertStatus(
        string value)
    {
        return TryParse(value, out var status)
            ? status
            : throw new InvalidOperationException($"Unknown status '{value}'");
    }
}
