using Domain.SeedWork;
using Domain.SeedWork.Errors;
using Domain.SeedWork.Result;

namespace LIMS.Service.Methodologies.Domain.StudyTemplateAggregate.States;

public sealed class ArchivedState : IState<StudyTemplate>
{
    public string Name => "Archived";
    public bool CanEdit => false;

    public Result<None, InvalidStatusTransitionError> CanTransitionTo(
        IState<StudyTemplate> newState,
        StudyTemplate template)
    {
        return newState switch
        {
            ArchivedState => new None(),
            _ => new InvalidStatusTransitionError(nameof(StudyTemplate), Name, newState.Name)
        };
    }
}
