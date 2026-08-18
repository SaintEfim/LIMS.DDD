using LIMS.Service.Methodologies.Domain.SeedWork;
using LIMS.Service.Methodologies.Domain.SeedWork.Result;

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
            ArchivedState => Result<None, Exception>.Success(new None()),
            _ => Result<None, Exception>.Failure(
                new InvalidOperationException("Archived templates cannot change status."))
        };
    }
}
