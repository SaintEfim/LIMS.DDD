using LIMS.DDD.Service.Domain.SeedWork.Result;

namespace LIMS.DDD.Service.Domain.StudyTemplateAggregate.Services;

public class StudyTemplateVersioningService
{
    public static Result<StudyTemplate, Exception> CreateNewRevisionAsync(
        StudyTemplate original,
        Revision newRevisionValue)
    {
        if (original.Status != Status.Active && original.Status != Status.Archived)
            return Result<StudyTemplate, Exception>.Failure(
                new InvalidOperationException("Can only create revisions from Active or Archived templates."));

        return StudyTemplate.Create(original.Name, original.Description, newRevisionValue)
            .Bind(newTemplate =>
            {
                newTemplate.SetParentId(original.Id);
                return Result<StudyTemplate, Exception>.Success(newTemplate);
            })
            .Bind(newTemplate =>
            {
                foreach (var param in original.InputParameters)
                {
                    var newSpecification = new Specification(
                        param.Specification.MinValue,
                        param.Specification.MaxValue);

                    var addResult = newTemplate.AddInputParameter(param.Name, param.Description, param.AliasName,
                        newSpecification);

                    if (addResult.IsFailure) return Result<StudyTemplate, Exception>.Failure(addResult.Error!);
                }

                foreach (var result in original.ResultDefinitions)
                {
                    var newSpecification = new Specification(
                        result.Specification.MinValue,
                        result.Specification.MaxValue);

                    var addResult = newTemplate.AddResultDefinition(
                        result.ResultInstance, result.Unit, newSpecification);

                    if (addResult.IsFailure) return Result<StudyTemplate, Exception>.Failure(addResult.Error!);
                }

                foreach (var rule in original.CalculationRules)
                {
                    var originalResultDef =
                        original.ResultDefinitions.FirstOrDefault(rd => rd.Id == rule.ResultDefinitionId);
                    var newResultDef = newTemplate.ResultDefinitions.FirstOrDefault(r =>
                        r.ResultInstance == originalResultDef?.ResultInstance);

                    if (newResultDef == null) continue;

                    var ruleResult = newTemplate.AddCalculationRule(rule.Name, rule.FormulaExpression, rule.Description,
                        newResultDef.Id);

                    if (ruleResult.IsFailure) return Result<StudyTemplate, Exception>.Failure(ruleResult.Error!);

                    var newRule = ruleResult.Value;

                    if (newRule is null) continue;

                    foreach (var input in rule.CalculationInputs)
                    {
                        var originalParam = original.InputParameters.First(ip => ip.Id == input.ParameterId);
                        var newParam =
                            newTemplate.InputParameters.FirstOrDefault(p => p.AliasName == originalParam.AliasName);

                        if (newParam is null) continue;

                        var inputResult = newTemplate.AddCalculationInput(newRule.Id, newParam.Id);
                        if (inputResult.IsFailure) return Result<StudyTemplate, Exception>.Failure(inputResult.Error!);
                    }
                }

                var archiveResult = original.ChangeStatus(Status.Archived);
                return archiveResult.IsFailure
                    ? Result<StudyTemplate, Exception>.Failure(archiveResult.Error!)
                    : Result<StudyTemplate, Exception>.Success(newTemplate);
            });
    }
}
