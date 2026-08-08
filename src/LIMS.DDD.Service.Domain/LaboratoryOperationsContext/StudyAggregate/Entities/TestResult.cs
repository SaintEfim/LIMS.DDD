using LIMS.DDD.Service.Domain.LaboratoryOperationsContext.StudyAggregate.ValueObjects;
using LIMS.DDD.Service.Domain.SeedWork;

namespace LIMS.DDD.Service.Domain.LaboratoryOperationsContext.StudyAggregate.Entities;

public sealed class TestResult : SoftDeletableModel
{
    private TestResult()
    {
    }

    public TestResultId Id { get; private set; }

    public StudyId StudyId { get; private set; }

    public ResultSnapshot ResultSnapshot { get; private set; } = null!;

    public double? Value { get; private set; }

    public bool IsOutOfSpec { get; private set; }

    internal static TestResult Create(
        StudyId studyId,
        ResultSnapshot snapshot)
    {
        return new TestResult
        {
            Id = new TestResultId(Guid.NewGuid()),
            StudyId = studyId,
            ResultSnapshot = snapshot
        };
    }

    public void SetValue(
        double value)
    {
        Value = value;
        RecalculateIsOutOfSpec();
    }

    private void RecalculateIsOutOfSpec()
    {
        if (!Value.HasValue)
        {
            IsOutOfSpec = false;
            return;
        }

        var isWithinSpec = ResultSnapshot.Specification.IsWithinSpec(Value.Value);
        IsOutOfSpec = !isWithinSpec;
    }

    internal void MarkAsDeleted()
    {
        IsDeleted = true;
        DeletedAt = DateTimeOffset.UtcNow;
    }
}
