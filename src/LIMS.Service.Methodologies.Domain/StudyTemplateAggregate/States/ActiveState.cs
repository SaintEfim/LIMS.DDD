using Domain.SeedWork;
using Domain.SeedWork.Result;

namespace LIMS.Service.Methodologies.Domain.StudyTemplateAggregate.States;

public sealed class ActiveState : IState<StudyTemplate>
{
    public string Name => "Active";
    public bool CanEdit => false;

    public Result<None, Exception> CanTransitionTo(
        IState<StudyTemplate> newState,
        StudyTemplate template)
    {
        return newState switch
        {
            ActiveState or ArchivedState => new None(),
            DraftState => new InvalidOperationException("Active templates cannot be reverted to Draft."),
            _ => new InvalidOperationException("Invalid transition from Active")
        };
    }
}
