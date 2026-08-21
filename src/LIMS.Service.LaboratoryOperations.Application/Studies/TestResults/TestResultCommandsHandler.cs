using Application.SeedWork.SeedWork;
using Domain.SeedWork.SeedWork;
using Domain.SeedWork.SeedWork.Result;
using Domain.SeedWork.SeedWork.ValueObjects;
using LIMS.Service.LaboratoryOperations.Domain.StudyAggregate;
using LIMS.Service.LaboratoryOperations.Domain.StudyAggregate.Entities;
using LIMS.Service.LaboratoryOperations.Domain.StudyTemplateSnapshots;
using LIMS.Service.LaboratoryOperations.Domain.StudyTemplateSnapshots.CalculationRules;
using NoStringEvaluating.Contract;
using NoStringEvaluating.Models.Values;

namespace LIMS.Service.LaboratoryOperations.Application.Studies.TestResults;

internal readonly record struct CalculationContext(
    TestResult Result,
    CalculationRuleSnapshot Rules,
    Dictionary<AliasName, double> Variables);

public sealed class TestResultCommandsHandler(
    IUnitOfWork unitOfWork,
    IStudyRepository studyRepository,
    IStudyTemplateSnapshotRepository studyTemplateRepository,
    INoStringEvaluator noStringEvaluator) : ICommandsHandler
{
    public async Task<Result<None, Exception>> ExecuteTest(
        Guid studyId,
        Guid testResultId,
        CancellationToken cancellationToken = default)
    {
        var prepareResult = await PrepareCalculationContext(studyId, testResultId, cancellationToken);
        if (prepareResult.IsFailure)
        {
            return prepareResult.CastFailure<None>();
        }

        var calcResult = CalculateFormula(prepareResult.GetValue());
        if (calcResult.IsFailure)
        {
            return calcResult.CastFailure<None>();
        }

        var (result, value) = calcResult.GetValue();
        result.SetValue(value);

        return await SaveChangesAsync(cancellationToken);
    }

    private async Task<Result<CalculationContext, Exception>> PrepareCalculationContext(
        Guid studyId,
        Guid testResultId,
        CancellationToken cancellationToken)
    {
        var studyResult = await GetStudyForChangeAsync(studyId, cancellationToken);
        if (studyResult.IsFailure)
        {
            return studyResult.GetError();
        }

        var study = studyResult.GetValue();

        var result = study.TestResults.FirstOrDefault(t => t.Id == new TestResultId(testResultId));
        if (result is null)
        {
            return new InvalidOperationException($"Test result '{testResultId}' not found in study '{studyId}'");
        }

        var template = await studyTemplateRepository.GetByIdAsync(study.StudyTemplateId, cancellationToken);
        if (template is null)
        {
            return new InvalidOperationException($"Study template '{study.StudyTemplateId}' not found");
        }

        var calculationRules =
            template.CalculationRules.FirstOrDefault(x => x.ResultDefinitionId == result.ResultDefinitionId);
        if (calculationRules is null)
        {
            return new InvalidOperationException(
                $"Calculation rule not found for result definition '{result.ResultDefinitionId}'");
        }

        var parametersByAlias = template.Parameters.ToDictionary(p => p.AliasName.Value, p => p);

        var measuredValuesByParamId = study.MeasuredValues.ToDictionary(m => m.InputParameterId, m => m);

        var extractVariables = calculationRules.FormulaExpression.ExtractVariables();
        var calculationOutputs = new Dictionary<AliasName, double>(extractVariables.Count);

        foreach (var variable in extractVariables)
        {
            if (!parametersByAlias.TryGetValue(variable, out var templateParameter))
            {
                return new InvalidOperationException($"Template parameter not found for formula variable '{variable}'");
            }

            if (!measuredValuesByParamId.TryGetValue(templateParameter.Id, out var measuredValue))
            {
                return new InvalidOperationException(
                    $"Measured value not found for parameter '{templateParameter.AliasName}'");
            }

            if (measuredValue.Value is null)
            {
                return new InvalidOperationException(
                    $"Missing required input parameter value for '{templateParameter.AliasName}'");
            }

            calculationOutputs.Add(templateParameter.AliasName, measuredValue.Value.Value);
        }

        var calculationContext = new CalculationContext(result, calculationRules, calculationOutputs);

        return calculationContext;
    }

    private Result<(TestResult Result, double Value), Exception> CalculateFormula(
        CalculationContext context)
    {
        try
        {
            var evaluatorValues = context.Variables.ToDictionary(x => x.Key.Value, x => new EvaluatorValue(x.Value));

            var calculatedValue = noStringEvaluator.CalcNumber(context.Rules.FormulaExpression.Value, evaluatorValues);

            return (context.Result, calculatedValue);
        }
        catch (Exception ex)
        {
            return new InvalidOperationException(
                $"Formula evaluation failed for result definition '{context.Result.ResultDefinitionId}': {ex.Message}",
                ex);
        }
    }

    private async Task<Result<Study, Exception>> GetStudyForChangeAsync(
        Guid studyId,
        CancellationToken ct)
    {
        var study = await studyRepository.GetByIdForChangeAsync(new StudyId(studyId), ct);
        return study is null ? new KeyNotFoundException($"Study with id {studyId} not found.") : study;
    }

    private async Task<Result<None, Exception>> SaveChangesAsync(
        CancellationToken cancellationToken = default)
    {
        try
        {
            await unitOfWork.SaveChangesAsync(cancellationToken);
            return new None();
        }
        catch (Exception ex)
        {
            return new Exception($"Failed to save changes: {ex.Message}", ex);
        }
    }

    public async Task<Result<None, Exception>> UpdateAsync(
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

        var updateResult = studyResult.GetValue()
            .UpdateTestResult(new TestResultId(testResultId), command.Value);

        if (updateResult.IsFailure)
        {
            return updateResult;
        }

        return await SaveChangesAsync(cancellationToken);
    }
}
