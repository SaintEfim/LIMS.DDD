using LIMS.DDD.Service.Domain.LaboratoryOperationsContext.SampleAggregate;
using LIMS.DDD.Service.Domain.LaboratoryOperationsContext.StudyAggregate.Entities;
using LIMS.DDD.Service.Domain.LaboratoryOperationsContext.StudyAggregate.ValueObjects;
using LIMS.DDD.Service.Domain.SeedWork;
using LIMS.DDD.Service.Domain.SeedWork.Result;
using LIMS.DDD.Service.Domain.StudyTemplateContext.StudyTemplateAggregate;

namespace LIMS.DDD.Service.Domain.LaboratoryOperationsContext.StudyAggregate;

public sealed class Study : IAggregateRoot
{
    private Study()
    {
    }

    public StudyId Id { get; private set; }

    public SampleId SampleId { get; private set; }

    public StudyTemplateId TemplateId { get; private set; }

    public StudyStatus Status { get; private set; } = StudyStatus.InWork;

    private readonly List<MeasuredValue> _measuredValues = [];

    public IReadOnlyList<MeasuredValue> MeasuredValues => _measuredValues.AsReadOnly();

    private readonly List<TestResult> _testResults = [];

    public IReadOnlyList<TestResult> TestResults => _testResults.AsReadOnly();

    public static Result<Study, Exception> Create(
        SampleId sampleId,
        StudyTemplateId templateId,
        IReadOnlyList<MeasuredValue> initialMeasuredValues,
        IReadOnlyList<TestResult> initialTestResults)
    {
        var study = new Study
        {
            Id = new StudyId(Guid.NewGuid()),
            SampleId = sampleId,
            TemplateId = templateId,
            Status = StudyStatus.InWork
        };

        study._measuredValues.AddRange(initialMeasuredValues);
        study._testResults.AddRange(initialTestResults);

        return Result<Study, Exception>.Success(study);
    }

    public Result<Exception> UpdateMeasuredValue(
        MeasuredValueId measuredValueId,
        double? value)
    {
        if (!Status.CanEdit)
            return Result<Exception>.Failure(
                new InvalidOperationException("Cannot update measured values when study is not InWork."));

        var measuredValue = _measuredValues.FirstOrDefault(mv => mv.Id == measuredValueId);

        if (measuredValue is null)
            return Result<Exception>.Failure(new InvalidOperationException("Measured value not found in this study."));

        measuredValue.Update(value);

        return Result<Exception>.Success();
    }

    public Result<Exception> UpdateTestResult(
        TestResultId testResultId,
        double? value,
        bool isOutOfSpec)
    {
        if (!Status.CanEdit)
            return Result<Exception>.Failure(
                new InvalidOperationException("Cannot update test results when study is not InWork."));

        var testResult = _testResults.FirstOrDefault(tr => tr.Id == testResultId);

        if (testResult is null)
            return Result<Exception>.Failure(new InvalidOperationException("Test result not found in this study."));

        testResult.Update(value, isOutOfSpec);

        return Result<Exception>.Success();
    }

    public Result<Exception> ChangeStatus(
        StudyStatus newStatus)
    {
        var result = Status.CanTransitionTo(newStatus, this);
        if (result.IsFailure) return result;

        Status = newStatus;
        return Result<Exception>.Success();
    }
}
