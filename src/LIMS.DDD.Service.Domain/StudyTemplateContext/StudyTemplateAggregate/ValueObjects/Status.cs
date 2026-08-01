using LIMS.DDD.Service.Domain.SeedWork.Result;
using LIMS.DDD.Service.Domain.StudyTemplateContext.StudyTemplateAggregate.States;

namespace LIMS.DDD.Service.Domain.StudyTemplateContext.StudyTemplateAggregate.ValueObjects;

public sealed record Status
{
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
        string name,
        out Status status)
    {
        return Registry.TryGetValue(name, out status!);
    }

    private readonly IState<StudyTemplate> _state;

    private Status(
        IState<StudyTemplate> state)
    {
        _state = state;
    }

    public string Name => _state.Name;
    public bool CanEdit => _state.CanEdit;

    public Result<Exception> CanTransitionTo(
        Status newStatus,
        StudyTemplate template)
    {
        return _state.CanTransitionTo(newStatus._state, template);
    }

    public override string ToString() => Name;

    public static Status ConvertStatus(string value)
    {
        if (TryParse(value, out var status))
            return status;

        throw new InvalidOperationException(
            $"Unknown status '{value}'");
    }
}
