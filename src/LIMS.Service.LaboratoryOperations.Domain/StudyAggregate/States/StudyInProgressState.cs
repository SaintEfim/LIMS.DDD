using LIMS.Service.LaboratoryOperations.Domain.SeedWork;
using LIMS.Service.LaboratoryOperations.Domain.SeedWork.Result;

namespace LIMS.Service.LaboratoryOperations.Domain.StudyAggregate.States;

public sealed class StudyInProgressState : IState<Study>
{
    public string Name => "InProgress";
    public bool CanEdit => true;

    public Result<None, Exception> CanTransitionTo(
        IState<Study> newState,
        Study study)
    {
        return newState switch
        {
            StudyInProgressState or StudyCompletedState => Result<None, Exception>.Success(new None()),
            _ => Result<None, Exception>.Failure(new InvalidOperationException("Invalid transition from InWork"))
        };
    }
}
