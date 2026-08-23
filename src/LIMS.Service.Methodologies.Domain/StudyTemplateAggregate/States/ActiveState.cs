using Domain.SeedWork;
using Domain.SeedWork.Errors;
using Domain.SeedWork.Result;

namespace LIMS.Service.Methodologies.Domain.StudyTemplateAggregate.States;

public sealed class ActiveState : IState<StudyTemplate>
{
    public string Name => "Active";
    public bool CanEdit => false;

    public Result<None, InvalidStatusTransitionError> CanTransitionTo(
        IState<StudyTemplate> newState,
        StudyTemplate template)
    {
        return newState switch
        {
            ActiveState or ArchivedState => Result<None, InvalidStatusTransitionError>.Success(),
            _ => Result<None, InvalidStatusTransitionError>.Failure(
                new InvalidStatusTransitionError(nameof(StudyTemplate), Name, newState.Name))
        };
    }
}
