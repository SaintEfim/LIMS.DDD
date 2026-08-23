using Application.SeedWork;
using Application.SeedWork.Errors;
using Domain.SeedWork;
using Domain.SeedWork.Result;
using Domain.SeedWork.ValueObjects;
using LIMS.Service.LaboratoryOperations.Domain.Services;
using LIMS.Service.LaboratoryOperations.Domain.StudyAggregate;
using LIMS.Service.LaboratoryOperations.Domain.StudyAggregate.Entities;
using LIMS.Service.LaboratoryOperations.Domain.StudyTemplateSnapshots;
using LIMS.Service.LaboratoryOperations.Domain.StudyTemplateSnapshots.CalculationRules;
using LIMS.Service.LaboratoryOperations.Domain.StudyTemplateSnapshots.ResultDefinitions;
using NoStringEvaluating.Contract;
using NoStringEvaluating.Models.Values;

namespace LIMS.Service.LaboratoryOperations.Application.Studies.TestResults;

internal readonly record struct CalculationContext(
    TestResult Result,
    ResultDefinitionSnapshot ResultDefinition,
    CalculationRuleSnapshot Rule,
    Dictionary<AliasName, double> Variables);

public sealed class TestResultCommandsHandler(
    IUnitOfWork unitOfWork,
    IStudyRepository studyRepository,
    IStudyTemplateSnapshotRepository studyTemplateRepository,
    INoStringEvaluator noStringEvaluator,
    TestResultDomainService testResultDomainService) : ICommandsHandler
{
    public async Task<Result<None, ApplicationError>> ExecuteTest(
        Guid studyId,
        Guid testResultId,
        CancellationToken cancellationToken = default)
    {
        var studyResult = await GetStudyForChangeAsync(studyId, cancellationToken);
        if (studyResult.IsFailure)
        {
            return studyResult.CastFailure<None>();
        }

        var study = studyResult.GetValue();

        var prepareResult = await PrepareCalculationContext(study, testResultId, cancellationToken);
        if (prepareResult.IsFailure)
        {
            return prepareResult.CastFailure<None>();
        }

        var context = prepareResult.GetValue();

        var calcResult = CalculateFormula(context);
        if (calcResult.IsFailure)
        {
            return calcResult.CastFailure<None>();
        }

        var calculatedValue = calcResult.GetValue();

        var setResult = testResultDomainService.SetValue(context.Result.Id, study, calculatedValue,
            context.ResultDefinition.Specification);

        if (setResult.IsFailure)
        {
            return new DomainRuleViolation(setResult.GetError());
        }

        return await SaveChangesAsync(cancellationToken);
    }

    public async Task<Result<None, ApplicationError>> UpdateAsync(
        Guid studyId,
        Guid testResultId,
        UpdateTestResultCommand command,
        CancellationToken cancellationToken = default)
    {
        var studyResult = await GetStudyForChangeAsync(studyId, cancellationToken);
        if (studyResult.IsFailure)
        {
            return studyResult.CastFailure<None>();
        }

        var study = studyResult.GetValue();

        var testResult = study.TestResults.FirstOrDefault(t => t.Id == new TestResultId(testResultId));
        if (testResult is null)
        {
            return new NotFoundError($"Test result '{testResultId}' not found in study.");
        }

        if (!command.Value.HasValue)
        {
            return new ValidationError("Value is required for update.");
        }

        var template = await studyTemplateRepository.GetByIdAsync(study.StudyTemplateId, cancellationToken);
        if (template is null)
        {
            return new NotFoundError($"Study template '{study.StudyTemplateId.Value}' not found.");
        }

        var resultDefinition = template.Results.FirstOrDefault(r => r.Id == testResult.ResultDefinitionId);
        if (resultDefinition is null)
        {
            return new NotFoundError(
                $"Result definition '{testResult.ResultDefinitionId.Value}' not found in snapshot.");
        }

        var setResult = testResultDomainService.SetValue(
            testResult.Id, study, command.Value.Value, resultDefinition.Specification);

        if (setResult.IsFailure)
        {
            return new DomainRuleViolation(setResult.GetError());
        }

        return await SaveChangesAsync(cancellationToken);
    }

    private async Task<Result<CalculationContext, ApplicationError>> PrepareCalculationContext(
        Study study,
        Guid testResultId,
        CancellationToken cancellationToken)
    {
        var testResult = study.TestResults.FirstOrDefault(t => t.Id == new TestResultId(testResultId));
        if (testResult is null)
        {
            return new NotFoundError($"Test result '{testResultId}' not found in study '{study.Id.Value}'.");
        }

        var template = await studyTemplateRepository.GetByIdAsync(study.StudyTemplateId, cancellationToken);
        if (template is null)
        {
            return new NotFoundError($"Study template '{study.StudyTemplateId.Value}' not found.");
        }

        var calculationRule =
            template.CalculationRules.FirstOrDefault(x => x.ResultDefinitionId == testResult.ResultDefinitionId);
        if (calculationRule is null)
        {
            return new NotFoundError(
                $"Calculation rule not found for result definition '{testResult.ResultDefinitionId.Value}'.");
        }

        var resultDefinition = template.Results.FirstOrDefault(r => r.Id == testResult.ResultDefinitionId);
        if (resultDefinition is null)
        {
            return new NotFoundError(
                $"Result definition '{testResult.ResultDefinitionId.Value}' not found in snapshot.");
        }

        var parametersByAlias = template.Parameters.ToDictionary(p => p.AliasName.Value, p => p);
        var measuredValuesByParamId = study.MeasuredValues.ToDictionary(m => m.InputParameterId, m => m);

        var extractVariables = calculationRule.FormulaExpression.ExtractVariables();
        var calculationOutputs = new Dictionary<AliasName, double>(extractVariables.Count);

        foreach (var variable in extractVariables)
        {
            if (!parametersByAlias.TryGetValue(variable, out var templateParameter))
            {
                return new NotFoundError($"Template parameter not found for formula variable '{variable}'.");
            }

            if (!measuredValuesByParamId.TryGetValue(templateParameter.Id, out var measuredValue))
            {
                return new NotFoundError(
                    $"Measured value not found for parameter '{templateParameter.AliasName.Value}'.");
            }

            if (measuredValue.Value is null)
            {
                return new ValidationError(
                    $"Missing required input parameter value for '{templateParameter.AliasName.Value}'.");
            }

            calculationOutputs.Add(templateParameter.AliasName, measuredValue.Value.Value);
        }

        return new CalculationContext(testResult, resultDefinition, calculationRule, calculationOutputs);
    }

    private Result<double, ApplicationError> CalculateFormula(
        CalculationContext context)
    {
        try
        {
            var evaluatorValues = context.Variables.ToDictionary(x => x.Key.Value, x => new EvaluatorValue(x.Value));

            var calculatedValue = noStringEvaluator.CalcNumber(context.Rule.FormulaExpression.Value, evaluatorValues);

            return calculatedValue;
        }
        catch (Exception ex)
        {
            return new ValidationError(
                $"Formula evaluation failed for result definition '{context.Result.ResultDefinitionId.Value}': {ex.Message}");
        }
    }

    private async Task<Result<None, ApplicationError>> SaveChangesAsync(
        CancellationToken cancellationToken = default)
    {
        try
        {
            await unitOfWork.SaveChangesAsync(cancellationToken);
            return new None();
        }
        catch (Exception ex)
        {
            return new PersistenceError($"Failed to save changes: {ex.Message}");
        }
    }

    private async Task<Result<Study, ApplicationError>> GetStudyForChangeAsync(
        Guid studyId,
        CancellationToken ct)
    {
        var study = await studyRepository.GetByIdForChangeAsync(new StudyId(studyId), ct);
        if (study is null)
        {
            return new NotFoundError($"Study with id '{studyId}' not found.");
        }

        return study;
    }
}
