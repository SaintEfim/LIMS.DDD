using LIMS.DDD.Service.Domain.SeedWork;
using LIMS.DDD.Service.Domain.SeedWork.Result;

namespace LIMS.DDD.Service.Domain.StudyTemplateContext.StudyTemplateAggregate.States;

public sealed class DraftState : IState<StudyTemplate>
{
    public string Name => "Draft";
    public bool CanEdit => true;

    public Result<Exception> CanTransitionTo(IState<StudyTemplate> newState, StudyTemplate template)
    {
        return newState switch
        {
            ActiveState => ValidateForActivation(template),
            DraftState or ArchivedState => Result<Exception>.Success(),
            _ => Result<Exception>.Failure(new InvalidOperationException("Invalid transition from Draft"))
        };
    }

    private static Result<Exception> ValidateForActivation(StudyTemplate template)
    {
        if (template.InputParameters.Count == 0)
            return Result<Exception>.Failure(new InvalidOperationException("Cannot activate without input parameters."));

        if (template.ResultDefinitions.Count == 0)
            return Result<Exception>.Failure(new InvalidOperationException("Cannot activate without result definitions."));

        if (template.CalculationRules.Count == 0)
            return Result<Exception>.Failure(new InvalidOperationException("Cannot activate without calculation rules."));

        return Result<Exception>.Success();
    }
}
