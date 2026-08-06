using LIMS.DDD.Service.Domain.SeedWork;
using LIMS.DDD.Service.Domain.SeedWork.Result;

namespace LIMS.DDD.Service.Domain.LaboratoryOperationsContext.StudyAggregate.States;

public sealed class StudyCanceledState : IState<Study>
{
    public string Name => "Canceled";

    public bool CanEdit => false;

    public Result<None, Exception> CanTransitionTo(
        IState<Study> newState,
        Study study)
    {
        return Result<None, Exception>.Failure(
            new InvalidOperationException("Canceled studies cannot change status."));
    }
}
