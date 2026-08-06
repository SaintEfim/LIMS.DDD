using LIMS.DDD.Service.Domain.SeedWork;
using LIMS.DDD.Service.Domain.SeedWork.Result;

namespace LIMS.DDD.Service.Domain.StudyTemplateContext.StudyTemplateAggregate.States;

public sealed class ArchivedState : IState<StudyTemplate>
{
    public string Name => "Archived";
    public bool CanEdit => false;

    public Result<UnitEmpty, Exception> CanTransitionTo(
        IState<StudyTemplate> newState,
        StudyTemplate template)
    {
        return newState switch
        {
            ArchivedState => Result<UnitEmpty, Exception>.Success(new UnitEmpty()),
            _ => Result<UnitEmpty, Exception>.Failure(
                new InvalidOperationException("Archived templates cannot change status."))
        };
    }
}
