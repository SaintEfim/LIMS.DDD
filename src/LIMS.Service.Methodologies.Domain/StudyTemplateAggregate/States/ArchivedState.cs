using Domain.SeedWork.SeedWork;
using Domain.SeedWork.SeedWork.Result;

namespace LIMS.Service.Methodologies.Domain.StudyTemplateAggregate.States;

public sealed class ArchivedState : IState<StudyTemplate>
{
    public string Name => "Archived";
    public bool CanEdit => false;

    public Result<None, Exception> CanTransitionTo(
        IState<StudyTemplate> newState,
        StudyTemplate template)
    {
        return newState switch
        {
            ArchivedState => new None(),
            _ => new InvalidOperationException("Archived templates cannot change status.")
        };
    }
}
