using LIMS.DDD.Service.Domain.SeedWork.Result;
using LIMS.DDD.Service.Domain.SeedWork.ValueObjects;
using LIMS.DDD.Service.Domain.StudyTemplateContext.StudyTemplateAggregate.Entities.InputParameters;
using LIMS.DDD.Service.Domain.StudyTemplateContext.StudyTemplateAggregate.Entities.ResultDefinitions;
using LIMS.DDD.Service.Domain.StudyTemplateContext.StudyTemplateAggregate.ValueObjects;

namespace LIMS.DDD.Service.Domain.StudyTemplateContext.StudyTemplateAggregate.Services;

public class StudyTemplateVersioningService
{
    public Result<StudyTemplate, Exception> CreateNewRevision(
        StudyTemplate original,
        Revision newRevisionValue)
    {
        if (original.Status != Status.Active && original.Status != Status.Archived)
        {
            return Result<StudyTemplate, Exception>.Failure(
                new InvalidOperationException("Can only create revisions from Active or Archived templates."));
        }

        var createResult = StudyTemplate.Create(original.Name, original.Description, newRevisionValue);
        if (createResult.IsFailure)
        {
            return createResult.CastFailure<StudyTemplate>();
        }

        var newTemplate = createResult.GetValue();
        newTemplate.SetParentId(original.Id);

        var copyResult = CopyChildren(original, newTemplate);
        if (copyResult.IsFailure)
        {
            return copyResult.CastFailure<StudyTemplate>();
        }

        if (original.Status != Status.Active)
        {
            return Result<StudyTemplate, Exception>.Success(newTemplate);
        }

        var archiveResult = original.ChangeStatus(Status.Archived);
        return archiveResult.IsFailure
            ? archiveResult.CastFailure<StudyTemplate>()
            : Result<StudyTemplate, Exception>.Success(newTemplate);
    }

    private static Result<None, Exception> CopyChildren(
        StudyTemplate original,
        StudyTemplate newTemplate)
    {
        var paramIdMap = new Dictionary<InputParameterId, InputParameterId>();
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

            paramIdMap[param.Id] = addResult.GetValue()
                .Id;
        }

        foreach (var result in original.ResultDefinitions)
        {
            var specResult = Specification.Create(result.Specification.MinValue, result.Specification.MaxValue);
            if (specResult.IsFailure)
            {
                return specResult.CastFailure<None>();
            }

            var addResult = newTemplate.AddResultDefinition(result.ResultInstance, result.Unit, specResult.GetValue());

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

            var newRule = ruleResult.GetValue();

            foreach (var input in rule.CalculationInputs)
            {
                if (!paramIdMap.TryGetValue(input.ParameterId, out var newParamId))
                {
                    continue;
                }

                var inputResult = newTemplate.AddCalculationInput(newRule.Id, newParamId);
                if (inputResult.IsFailure)
                {
                    return inputResult.CastFailure<None>();
                }
            }
        }

        return Result<None, Exception>.Success();
    }
}
