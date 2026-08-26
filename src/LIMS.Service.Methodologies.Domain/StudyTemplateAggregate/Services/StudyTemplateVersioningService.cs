using Domain.SeedWork.Errors;
using Domain.SeedWork.Result;
using Domain.SeedWork.ValueObjects;
using LIMS.Service.Methodologies.Domain.StudyTemplateAggregate.Entities.ResultDefinitions;
using LIMS.Service.Methodologies.Domain.StudyTemplateAggregate.ValueObjects;

namespace LIMS.Service.Methodologies.Domain.StudyTemplateAggregate.Services;

public class StudyTemplateVersioningService
{
    public Result<StudyTemplate, DomainError> CreateNewRevision(
        StudyTemplate original,
        Revision newRevisionValue)
    {
        if (original.Status != Status.Active && original.Status != Status.Archived)
        {
            return Result<StudyTemplate, DomainError>.Failure(
                new InvalidStatusTransitionError(nameof(StudyTemplate), original.Status.Name, "Revision"));
        }

        var newTemplate = new StudyTemplate(original.Name, original.Description, newRevisionValue);

        newTemplate.SetParentId(original.Id);

        var copyResult = CopyChildren(original, newTemplate);
        if (copyResult.IsFailure)
        {
            return copyResult.CastFailure<StudyTemplate>();
        }

        if (original.Status != Status.Active)
        {
            return newTemplate;
        }

        var archiveResult = original.ChangeStatus(Status.Archived);
        return archiveResult.IsFailure ? archiveResult.CastFailure<StudyTemplate>() : newTemplate;
    }

    private static Result<None, DomainError> CopyChildren(
        StudyTemplate original,
        StudyTemplate newTemplate)
    {
        var resultDefIdMap = new Dictionary<ResultDefinitionId, ResultDefinitionId>();

        foreach (var param in original.InputParameters)
        {
            var specResult = Specification.Create(param.Specification.MinValue, param.Specification.MaxValue);
            if (specResult.IsFailure)
            {
                return specResult.CastFailure<None>();
            }

            var addResult = newTemplate.AddInputParameter(
                param.Name, param.Description, param.AliasName, specResult.GetValue());

            if (addResult.IsFailure)
            {
                return addResult.CastFailure<None>();
            }
        }

        foreach (var result in original.ResultDefinitions)
        {
            var specResult = Specification.Create(result.Specification.MinValue, result.Specification.MaxValue);
            if (specResult.IsFailure)
            {
                return specResult.CastFailure<None>();
            }

            var addResult =
                newTemplate.AddResultDefinition(result.ResultInstance, result.UnitId, specResult.GetValue());

            if (addResult.IsFailure)
            {
                return addResult.CastFailure<None>();
            }

            resultDefIdMap[result.Id] = addResult.GetValue()
                .Id;
        }

        foreach (var rule in original.CalculationRules)
        {
            if (!resultDefIdMap.TryGetValue(rule.ResultDefinitionId, out var newResultDefId))
            {
                continue;
            }

            var ruleResult = newTemplate.AddCalculationRule(
                rule.Name, rule.FormulaExpression, rule.Description, newResultDefId);

            if (ruleResult.IsFailure)
            {
                return ruleResult.CastFailure<None>();
            }
        }

        return new None();
    }
}
