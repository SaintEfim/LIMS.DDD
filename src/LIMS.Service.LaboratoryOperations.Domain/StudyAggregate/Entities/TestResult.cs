using Domain.SeedWork.SeedWork;
using LIMS.Service.LaboratoryOperations.Domain.StudyTemplateSnapshots.ResultDefinitions;

namespace LIMS.Service.LaboratoryOperations.Domain.StudyAggregate.Entities;

public sealed class TestResult : SoftDeletableModel
{
    internal TestResult(
        StudyId studyId,
        ResultDefinitionId resultDefinitionId)
    {
        Id = new TestResultId(Guid.NewGuid());
        StudyId = studyId;
        ResultDefinitionId = resultDefinitionId;
    }

    public TestResultId Id { get; private set; }

    public StudyId StudyId { get; private set; }

    public ResultDefinitionId ResultDefinitionId { get; private set; }

    public double? Value { get; private set; }

    public bool IsOutOfSpec { get; private set; }

    internal void ApplyValue(
        double value,
        bool isOutOfSpec)
    {
        Value = value;
        IsOutOfSpec = isOutOfSpec;
    }

    internal void MarkAsDeleted()
    {
        IsDeleted = true;
        DeletedAt = DateTimeOffset.UtcNow;
    }
}
