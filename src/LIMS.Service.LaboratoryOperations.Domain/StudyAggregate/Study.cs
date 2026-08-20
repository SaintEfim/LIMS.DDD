using Domain.SeedWork.SeedWork;
using Domain.SeedWork.SeedWork.Result;
using Domain.SeedWork.SeedWork.SoftDeletable;
using Domain.SeedWork.SeedWork.ValueObjects;
using LIMS.Service.LaboratoryOperations.Domain.SampleAggregate;
using LIMS.Service.LaboratoryOperations.Domain.StudyAggregate.Entities;
using LIMS.Service.LaboratoryOperations.Domain.StudyAggregate.ValueObjects;
using LIMS.Service.LaboratoryOperations.Domain.StudyTemplateSnapshots;

namespace LIMS.Service.LaboratoryOperations.Domain.StudyAggregate;

public sealed class Study
    : SoftDeletableModel,
        IAggregateRoot
{
    private readonly List<MeasuredValue> _measuredValues = [];

    private readonly List<TestResult> _testResults = [];

    internal Study(
        StudyId studyId,
        SampleId sampleId,
        Name templateName,
        StudyTemplateId templateId,
        IReadOnlyList<MeasuredValue> initialMeasuredValues,
        IReadOnlyList<TestResult> initialTestResults)
    {
        Id = studyId;
        SampleId = sampleId;
        Name = templateName;
        StudyTemplateId = templateId;
        Description = Description.Create(null)
            .GetValue();
        Status = StudyStatus.InProgress;

        _measuredValues.AddRange(initialMeasuredValues);
        _testResults.AddRange(initialTestResults);
    }

    public StudyId Id { get; private set; }

    public SampleId SampleId { get; private set; }

    public StudyStatus Status { get; private set; }

    public Name Name { get; private set; }

    public StudyTemplateId StudyTemplateId { get; private set; }

    public Description Description { get; private set; }

    public IReadOnlyList<MeasuredValue> MeasuredValues => _measuredValues.AsReadOnly();
    public IReadOnlyList<TestResult> TestResults => _testResults.AsReadOnly();

    public Result<None, Exception> UpdateNotes(
        Description? description)
    {
        if (!Status.CanEdit)
        {
            return new InvalidOperationException("Cannot update study notes when study is not InWork.");
        }

        if (description is not null)
        {
            Description = description;
        }

        return new None();
    }

    public Result<None, Exception> ReassignSample(
        SampleId newSampleId)
    {
        if (!Status.CanEdit)
        {
            return new InvalidOperationException("Cannot reassign sample when study is not InWork.");
        }

        if (newSampleId == SampleId)
        {
            return new None();
        }

        SampleId = newSampleId;
        return new None();
    }

    public Result<None, Exception> UpdateTestResult(
        TestResultId testResultId,
        double? value)
    {
        if (!Status.CanEdit)
        {
            return new InvalidOperationException("Cannot update test results when study is not InWork.");
        }

        var testResult = _testResults.FirstOrDefault(tr => tr.Id == testResultId);

        if (testResult is null)
        {
            return new InvalidOperationException("Test result not found in this study.");
        }

        if (value is not null)
        {
            testResult.SetValue(value.Value);
        }

        return new None();
    }

    public Result<None, Exception> UpdateMeasuredValue(
        MeasuredValueId measuredValueId,
        double? value)
    {
        if (!Status.CanEdit)
        {
            return new InvalidOperationException("Cannot update measured values when study is not InWork.");
        }

        var measuredValue = _measuredValues.FirstOrDefault(mv => mv.Id == measuredValueId);

        if (measuredValue is null)
        {
            return new InvalidOperationException("Measured value not found in this study.");
        }

        measuredValue.Update(value);

        return new None();
    }

    internal Result<None, Exception> ChangeStatus(
        StudyStatus newStatus)
    {
        var result = Status.CanTransitionTo(newStatus, this);
        if (result.IsFailure)
        {
            return result.CastFailure<None>();
        }

        Status = newStatus;
        return new None();
    }

    public Result<None, Exception> Delete()
    {
        if (IsDeleted)
        {
            return new InvalidOperationException("Study is already deleted.");
        }

        if (Status == StudyStatus.Completed || Status == StudyStatus.Approved)
        {
            return new InvalidOperationException(
                "Cannot delete a Completed or Approved study. Use 'Cancel' status instead.");
        }

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

        return new None();
    }
}
