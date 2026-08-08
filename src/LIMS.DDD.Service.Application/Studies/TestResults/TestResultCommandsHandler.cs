using LIMS.DDD.Service.Domain.LaboratoryOperationsContext.StudyAggregate;
using LIMS.DDD.Service.Domain.LaboratoryOperationsContext.StudyAggregate.Entities;
using LIMS.DDD.Service.Domain.SeedWork;
using LIMS.DDD.Service.Domain.SeedWork.Result;
using LIMS.DDD.Service.Domain.SeedWork.ValueObjects;
using LIMS.DDD.Service.Domain.StudyTemplateContext.StudyTemplateAggregate;
using LIMS.DDD.Service.Domain.StudyTemplateContext.StudyTemplateAggregate.Entities.ResultDefinitions;
using NoStringEvaluating.Contract;
using NoStringEvaluating.Models.Values;

namespace LIMS.DDD.Service.Application.Studies.TestResults;

public sealed class TestResultCommandsHandler(
    IUnitOfWork unitOfWork,
    IStudyRepository studyRepository,
    IStudyTemplateRepository studyTemplateRepository,
    INoStringEvaluator noStringEvaluator)
{
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
            .UpdateTestResult(new TestResultId(testResultId), command.IsOutOfSpec);

        if (updateResult.IsFailure)
        {
            return updateResult;
        }

        return await SaveChangesAsync(cancellationToken);
    }

    public async Task<Result<None, Exception>> ExecuteTest(
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

        var result = study.TestResults.SingleOrDefault(t => t.Id == new TestResultId(testResultId));
        if (result is null) return Result<None, Exception>.Failure(new Exception("Not found result"));

        var template =
            await studyTemplateRepository.GetByIdAsync(new StudyTemplateId(study.TemplateId.Value), cancellationToken);

        var calculationRules = template?.CalculationRules.FirstOrDefault(x =>
            x.ResultDefinitionId == new ResultDefinitionId(result.ResultSnapshot.ResultDefinitionId));

        var calculationInputs =
            calculationRules.CalculationInputs.ToDictionary(x => x.ParameterId.Value, x => x.VariableAlias);

        var calculationOutputs = new Dictionary<AliasName, double>();

        foreach (var templateParameterId in calculationInputs.Keys)
        {
            var parameter =
                study.MeasuredValues.FirstOrDefault(x => x.ParameterSnapshot.InputParameterId == templateParameterId);

            if (parameter != null)
            {
                // перед этим мы проверили что все параметры заполнены
                calculationOutputs.Add(calculationInputs[templateParameterId], parameter.Value.Value);
            }
        }

        var dictEvaluatorValue = calculationOutputs.ToDictionary(x => x.Key.Value, x => new EvaluatorValue(x.Value));

        var resFormula = noStringEvaluator.CalcNumber(calculationRules.FormulaExpression.Value, dictEvaluatorValue);

        result.SetValue(resFormula);

        return await SaveChangesAsync(cancellationToken);
    }

    private async Task<Result<Study, Exception>> GetStudyForChangeAsync(
        Guid studyId,
        CancellationToken ct)
    {
        var study = await studyRepository.GetByIdForChangeAsync(new StudyId(studyId), ct);
        return study is null
            ? Result<Study, Exception>.Failure(new KeyNotFoundException($"Study with id {studyId} not found."))
            : Result<Study, Exception>.Success(study);
    }

    private async Task<Result<None, Exception>> SaveChangesAsync(
        CancellationToken cancellationToken)
    {
        try
        {
            await unitOfWork.SaveChangesAsync(cancellationToken);
            return Result<None, Exception>.Success();
        }
        catch (Exception ex)
        {
            return Result<None, Exception>.Failure(new Exception($"Failed to save changes: {ex.Message}", ex));
        }
    }
}
