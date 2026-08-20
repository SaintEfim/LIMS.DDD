using Domain.SeedWork.SeedWork;
using Domain.SeedWork.SeedWork.Result;

namespace LIMS.Service.Methodologies.Domain.StudyTemplateAggregate.States;

public sealed class DraftState : IState<StudyTemplate>
{
    public string Name => "Draft";
    public bool CanEdit => true;

    public Result<None, Exception> CanTransitionTo(
        IState<StudyTemplate> newState,
        StudyTemplate template)
    {
        return newState switch
        {
            ActiveState => ValidateForActivation(template),
            DraftState or ArchivedState => new None(),
            _ => new InvalidOperationException("Invalid transition from Draft")
        };
    }

    private static Result<None, Exception> ValidateForActivation(
        StudyTemplate template)
    {
        if (template.InputParameters.Count == 0)
        {
            return new InvalidOperationException("Cannot activate without input parameters.");
        }

        if (template.ResultDefinitions.Count == 0)
        {
            return new InvalidOperationException("Cannot activate without result definitions.");
        }

        if (template.CalculationRules.Count == 0)
        {
            return new InvalidOperationException("Cannot activate without calculation rules.");
        }

        return new None();
    }
}
