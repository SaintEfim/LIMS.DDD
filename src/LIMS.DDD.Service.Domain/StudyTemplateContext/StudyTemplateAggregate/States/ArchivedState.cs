using LIMS.DDD.Service.Domain.SeedWork;
using LIMS.DDD.Service.Domain.SeedWork.Result;

namespace LIMS.DDD.Service.Domain.StudyTemplateContext.StudyTemplateAggregate.States;

public sealed class ArchivedState : IState<StudyTemplate>
{
    public string Name => "Archived";
    public bool CanEdit => false;

    public Result<Exception> CanTransitionTo(IState<StudyTemplate> newState, StudyTemplate template)
    {
        return newState switch
        {
            ArchivedState => Result<Exception>.Success(),
            _ => Result<Exception>.Failure(new InvalidOperationException("Archived templates cannot change status."))
        };
    }
}
