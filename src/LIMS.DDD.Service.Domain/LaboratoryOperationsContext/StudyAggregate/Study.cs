using LIMS.DDD.Service.Domain.LaboratoryOperationsContext.SampleAggregate;
using LIMS.DDD.Service.Domain.LaboratoryOperationsContext.StudyAggregate.Entities;
using LIMS.DDD.Service.Domain.LaboratoryOperationsContext.StudyAggregate.ValueObjects;
using LIMS.DDD.Service.Domain.SeedWork;
using LIMS.DDD.Service.Domain.SeedWork.Result;
using LIMS.DDD.Service.Domain.SeedWork.ValueObjects;

namespace LIMS.DDD.Service.Domain.LaboratoryOperationsContext.StudyAggregate;

public sealed class Study
    : SoftDeletableModel,
        IAggregateRoot
{
    private Study()
    {
    }

    public StudyId Id { get; private set; }

    public SampleId SampleId { get; private set; }

    public StudyStatus Status { get; private set; } = StudyStatus.InWork;

    public Name Name { get; private set; }

    public TemplateId TemplateId { get; private set; }

    public Description Description { get; private set; }

    private readonly List<MeasuredValue> _measuredValues = [];
    public IReadOnlyList<MeasuredValue> MeasuredValues => _measuredValues.AsReadOnly();

    private readonly List<TestResult> _testResults = [];
    public IReadOnlyList<TestResult> TestResults => _testResults.AsReadOnly();

    internal static Result<Study, Exception> Create(
        StudyId studyId,
        SampleId sampleId,
        Name templateName,
        TemplateId templateId,
        IReadOnlyList<MeasuredValue> initialMeasuredValues,
        IReadOnlyList<TestResult> initialTestResults)
    {
        var study = new Study
        {
            Id = studyId,
            SampleId = sampleId,
            Name = templateName,
            TemplateId = templateId,
            Status = StudyStatus.InWork
        };

        study._measuredValues.AddRange(initialMeasuredValues);
        study._testResults.AddRange(initialTestResults);

        return Result<Study, Exception>.Success(study);
    }

    public Result<Exception> UpdateNotes(
        Description? description)
    {
        if (!Status.CanEdit)
            return Result<Exception>.Failure(
                new InvalidOperationException("Cannot update study notes when study is not InWork."));

        if (description is not null) Description = description;
        return Result<Exception>.Success();
    }

    public Result<Exception> ReassignSample(
        SampleId newSampleId)
    {
        if (!Status.CanEdit)
            return Result<Exception>.Failure(
                new InvalidOperationException("Cannot reassign sample when study is not InWork."));

        if (newSampleId == SampleId) return Result<Exception>.Success();

        SampleId = newSampleId;
        return Result<Exception>.Success();
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

    public Result<Exception> Delete()
    {
        if (IsDeleted) return Result<Exception>.Failure(new InvalidOperationException("Study is already deleted."));

        if (Status == StudyStatus.Completed || Status == StudyStatus.Approved)
            return Result<Exception>.Failure(
                new InvalidOperationException(
                    "Cannot delete a Completed or Approved study. Use 'Cancel' status instead."));

        IsDeleted = true;
        DeletedAt = DateTimeOffset.UtcNow;

        foreach (var measuredValue in _measuredValues)
        {
            measuredValue.MarkAsDeleted();
        }

        foreach (var testResult in _testResults)
        {
            testResult.MarkAsDeleted();
        }

        return Result<Exception>.Success();
    }
}
