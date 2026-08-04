using LIMS.DDD.Service.Domain.SeedWork.Result;
using LIMS.DDD.Service.Domain.StudyTemplateContext.StudyTemplateAggregate.Entities.InputParameters;
using LIMS.DDD.Service.Domain.StudyTemplateContext.StudyTemplateAggregate.Entities.ResultDefinitions;
using LIMS.DDD.Service.Domain.StudyTemplateContext.StudyTemplateAggregate.ValueObjects;

namespace LIMS.DDD.Service.Domain.StudyTemplateContext.StudyTemplateAggregate.Services;

public record RevisionCreationResult(StudyTemplate Original, StudyTemplate NewTemplate);

public class StudyTemplateVersioningService
{
    public Result<RevisionCreationResult, Exception> CreateNewRevision(
        StudyTemplate original,
        Revision newRevisionValue)
    {
        if (original.Status != Status.Active && original.Status != Status.Archived)
            return Result<RevisionCreationResult, Exception>.Failure(
                new InvalidOperationException("Can only create revisions from Active or Archived templates."));

        var createResult = StudyTemplate.Create(original.Name, original.Description, newRevisionValue);
        if (createResult.IsFailure) return Result<RevisionCreationResult, Exception>.Failure(createResult.Error!);

        var newTemplate = createResult.GetValue();
        newTemplate.SetParentId(original.Id);

        var copyResult = CopyChildren(original, newTemplate);
        if (copyResult.IsFailure) return Result<RevisionCreationResult, Exception>.Failure(copyResult.Error!);

        if (original.Status != Status.Active)
        {
            return Result<RevisionCreationResult, Exception>.Success(new RevisionCreationResult(original, newTemplate));
        }

        var archiveResult = original.ChangeStatus(Status.Archived);
        return archiveResult.IsFailure
            ? Result<RevisionCreationResult, Exception>.Failure(archiveResult.Error!)
            : Result<RevisionCreationResult, Exception>.Success(new RevisionCreationResult(original, newTemplate));
    }

    private static Result<Exception> CopyChildren(
        StudyTemplate original,
        StudyTemplate newTemplate)
    {
        var paramIdMap = new Dictionary<InputParameterId, InputParameterId>();
        var resultDefIdMap = new Dictionary<ResultDefinitionId, ResultDefinitionId>();

        foreach (var param in original.InputParameters)
        {
            var specResult = Specification.Create(param.Specification.MinValue, param.Specification.MaxValue);
            if (specResult.IsFailure) return Result<Exception>.Failure(specResult.Error!);

            var addResult = newTemplate.AddInputParameter(
                param.Name,
                param.Description,
                param.AliasName,
                specResult.GetValue());

            if (addResult.IsFailure) return Result<Exception>.Failure(addResult.Error!);

            paramIdMap[param.Id] = addResult.GetValue().Id;
        }

        foreach (var result in original.ResultDefinitions)
        {
            var specResult = Specification.Create(result.Specification.MinValue, result.Specification.MaxValue);
            if (specResult.IsFailure) return Result<Exception>.Failure(specResult.Error!);

            var addResult = newTemplate.AddResultDefinition(
                result.ResultInstance,
                result.Unit,
                specResult.GetValue());

            if (addResult.IsFailure) return Result<Exception>.Failure(addResult.Error!);

            resultDefIdMap[result.Id] = addResult.GetValue().Id;
        }

        foreach (var rule in original.CalculationRules)
        {
            if (!resultDefIdMap.TryGetValue(rule.ResultDefinitionId, out var newResultDefId))
                continue;

            var ruleResult = newTemplate.AddCalculationRule(
                rule.Name,
                rule.FormulaExpression,
                rule.Description,
                newResultDefId);

            if (ruleResult.IsFailure) return Result<Exception>.Failure(ruleResult.Error!);

            var newRule = ruleResult.GetValue();

            foreach (var input in rule.CalculationInputs)
            {
                if (!paramIdMap.TryGetValue(input.ParameterId, out var newParamId))
                    continue;

                var inputResult = newTemplate.AddCalculationInput(newRule.Id, newParamId);
                if (inputResult.IsFailure) return Result<Exception>.Failure(inputResult.Error!);
            }
        }

        return Result<Exception>.Success();
    }
}
