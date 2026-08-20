using Domain.SeedWork.SeedWork;
using Domain.SeedWork.SeedWork.Result;

namespace LIMS.Service.LaboratoryOperations.Domain.StudyAggregate.States;

public sealed class StudyCanceledState : IState<Study>
{
    public string Name => "Canceled";

    public bool CanEdit => false;

    public Result<None, Exception> CanTransitionTo(
        IState<Study> newState,
        Study study)
    {
        return Result<None, Exception>.Failure(new InvalidOperationException("Canceled studies cannot change status."));
    }
}
