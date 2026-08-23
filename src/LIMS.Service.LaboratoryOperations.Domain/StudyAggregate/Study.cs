using Domain.SeedWork;
using Domain.SeedWork.Errors;
using Domain.SeedWork.Result;
using Domain.SeedWork.ValueObjects;
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

    // for EF Core
    private Study()
    {
    }

    public StudyId Id { get; }

    public SampleId SampleId { get; private set; }

    public StudyStatus Status { get; private set; } = null!;

    public Name Name { get; private set; } = null!;

    public StudyTemplateId StudyTemplateId { get; private set; }

    public Description Description { get; private set; } = null!;

    public IReadOnlyList<MeasuredValue> MeasuredValues => _measuredValues.AsReadOnly();

    public IReadOnlyList<TestResult> TestResults => _testResults.AsReadOnly();

    public Result<None, DomainError> UpdateNotes(
        Description? description)
    {
        if (!Status.CanEdit)
        {
            return new EntityNotEditableError(nameof(Study), Status.Name, "update study notes");
        }

        if (description is not null)
        {
            Description = description;
        }

        return new None();
    }

    public Result<None, DomainError> ReassignSample(
        SampleId newSampleId)
    {
        if (!Status.CanEdit)
        {
            return new EntityNotEditableError(nameof(Study), Status.Name, "reassign sample");
        }

        if (newSampleId == SampleId)
        {
            return new None();
        }

        SampleId = newSampleId;

        return new None();
    }

    public Result<None, DomainError> UpdateTestResult(
        TestResultId testResultId,
        double? value,
        bool isWithinSpec)
    {
        if (!Status.CanEdit)
        {
            return new EntityNotEditableError(nameof(Study), Status.Name, "update test results");
        }

        var testResult = _testResults.FirstOrDefault(tr => tr.Id == testResultId);
        if (testResult is null)
        {
            return new EntityNotFoundError("Test result", testResultId.Value);
        }

        if (value is not null)
        {
            testResult.ApplyValue(value.Value, !isWithinSpec);
        }

        return new None();
    }

    public Result<None, DomainError> UpdateMeasuredValue(
        MeasuredValueId measuredValueId,
        double? value)
    {
        if (!Status.CanEdit)
        {
            return new EntityNotEditableError(nameof(Study), Status.Name, "update measured values");
        }

        var measuredValue = _measuredValues.FirstOrDefault(mv => mv.Id == measuredValueId);
        if (measuredValue is null)
        {
            return new EntityNotFoundError("Measured value", measuredValueId.Value);
        }

        measuredValue.Update(value);

        return new None();
    }

    internal Result<None, InvalidStatusTransitionError> ChangeStatus(
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

    public Result<None, DomainError> Delete()
    {
        if (IsDeleted)
        {
            return new EntityAlreadyDeletedError(nameof(Study), Id.Value);
        }

        if (Status == StudyStatus.Completed || Status == StudyStatus.Approved)
        {
            return new InvalidStatusTransitionError(nameof(Study), Status.Name, "Deleted");
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
