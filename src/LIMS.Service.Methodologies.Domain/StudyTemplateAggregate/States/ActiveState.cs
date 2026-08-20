using Domain.SeedWork.SeedWork;
using Domain.SeedWork.SeedWork.Result;

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
            ActiveState or ArchivedState => Result<None, Exception>.Success(new None()),
            DraftState => Result<None, Exception>.Failure(
                new InvalidOperationException("Active templates cannot be reverted to Draft.")),
            _ => Result<None, Exception>.Failure(new InvalidOperationException("Invalid transition from Active"))
        };
    }
}
