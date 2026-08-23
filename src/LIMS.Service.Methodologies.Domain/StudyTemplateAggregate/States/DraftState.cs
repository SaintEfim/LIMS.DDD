using Domain.SeedWork;
using Domain.SeedWork.Errors;
using Domain.SeedWork.Result;

namespace LIMS.Service.Methodologies.Domain.StudyTemplateAggregate.States;

public sealed class DraftState : IState<StudyTemplate>
{
    public string Name => "Draft";
    public bool CanEdit => true;

    public Result<None, InvalidStatusTransitionError> CanTransitionTo(
        IState<StudyTemplate> newState,
        StudyTemplate template)
    {
        return newState switch
        {
            ActiveState => ValidateForActivation(newState, template),
            DraftState or ArchivedState => new None(),
            _ => new InvalidStatusTransitionError(nameof(StudyTemplate), Name, newState.Name)
        };
    }

    private Result<None, InvalidStatusTransitionError> ValidateForActivation(
        IState<StudyTemplate> newState,
        StudyTemplate template)
    {
        if (template.InputParameters.Count == 0)
        {
            return Result<None, InvalidStatusTransitionError>.Failure(
                new InvalidStatusTransitionError(nameof(StudyTemplate), Name, newState.Name,
                    "Cannot activate without input parameters."));
        }

        if (template.ResultDefinitions.Count == 0)
        {
            return Result<None, InvalidStatusTransitionError>.Failure(
                new InvalidStatusTransitionError(nameof(StudyTemplate), Name, newState.Name,
                    "Cannot activate without result definitions."));
        }

        if (template.CalculationRules.Count == 0)
        {
            return Result<None, InvalidStatusTransitionError>.Failure(
                new InvalidStatusTransitionError(nameof(StudyTemplate), Name, newState.Name,
                    "Cannot activate without calculation rules."));
        }

        return new None();
    }
}
