using Domain.SeedWork.SeedWork;
using Domain.SeedWork.SeedWork.ValueObjects;
using LIMS.Service.Methodologies.Domain.StudyTemplateAggregate.States;

namespace LIMS.Service.Methodologies.Domain.StudyTemplateAggregate.ValueObjects;

public sealed record Status : StatusBase<IState<StudyTemplate>, StudyTemplate>
{
    private Status(
        IState<StudyTemplate> state)
        : base(state)
    {
    }

    public static Status Draft { get; } = new(new DraftState());
    public static Status Active { get; } = new(new ActiveState());
    public static Status Archived { get; } = new(new ArchivedState());

    private static readonly Dictionary<string, Status> Registry =
        new(StringComparer.OrdinalIgnoreCase)
        {
            ["Draft"] = Draft,
            ["Active"] = Active,
            ["Archived"] = Archived
        };

    public static bool TryParse(
        string? name,
        out Status? status)
    {
        status = null;
        return name is not null && Registry.TryGetValue(name, out status);
    }

    public static Status ConvertStatus(
        string? value)
    {
        return TryParse(value, out var status) && status is not null
            ? status
            : throw new InvalidOperationException($"Unknown status '{value}'");
    }
}
