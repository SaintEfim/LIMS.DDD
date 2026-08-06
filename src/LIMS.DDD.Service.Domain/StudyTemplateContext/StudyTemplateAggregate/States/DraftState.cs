using LIMS.DDD.Service.Domain.SeedWork;
using LIMS.DDD.Service.Domain.SeedWork.Result;

namespace LIMS.DDD.Service.Domain.StudyTemplateContext.StudyTemplateAggregate.States;

public sealed class DraftState : IState<StudyTemplate>
{
    public string Name => "Draft";
    public bool CanEdit => true;

    public Result<UnitEmpty, Exception> CanTransitionTo(
        IState<StudyTemplate> newState,
        StudyTemplate template)
    {
        return newState switch
        {
            ActiveState => ValidateForActivation(template),
            DraftState or ArchivedState => Result<UnitEmpty, Exception>.Success(new UnitEmpty()),
            _ => Result<UnitEmpty, Exception>.Failure(new InvalidOperationException("Invalid transition from Draft"))
        };
    }

    private static Result<UnitEmpty, Exception> ValidateForActivation(
        StudyTemplate template)
    {
        if (template.InputParameters.Count == 0)
        {
            return Result<UnitEmpty, Exception>.Failure(
                new InvalidOperationException("Cannot activate without input parameters."));
        }

        if (template.ResultDefinitions.Count == 0)
        {
            return Result<UnitEmpty, Exception>.Failure(
                new InvalidOperationException("Cannot activate without result definitions."));
        }

        if (template.CalculationRules.Count == 0)
        {
            return Result<UnitEmpty, Exception>.Failure(
                new InvalidOperationException("Cannot activate without calculation rules."));
        }

        return Result<UnitEmpty, Exception>.Success(new UnitEmpty());
    }
}
