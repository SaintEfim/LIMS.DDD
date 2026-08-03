using LIMS.DDD.Service.Domain.StudyTemplateContext.StudyTemplateAggregate.Entities.ResultDefinitions;

namespace LIMS.DDD.Service.Domain.LaboratoryOperationsContext.StudyAggregate.Entities;

public sealed class TestResult
{
    private TestResult()
    {
    }

    public TestResultId Id { get; private set; }

    public StudyId StudyId { get; private set; }

    public ResultDefinitionId ResultDefinitionId { get; private set; }

    public double? Value { get; private set; }

    public bool IsOutOfSpec { get; private set; }

    public static TestResult Create(
        StudyId studyId,
        ResultDefinitionId resultDefinitionId,
        double? value,
        bool isOutOfSpec)
    {
        return new TestResult
        {
            Id = new TestResultId(Guid.NewGuid()),
            StudyId = studyId,
            ResultDefinitionId = resultDefinitionId,
            Value = value,
            IsOutOfSpec = isOutOfSpec
        };
    }

    public void Update(
        double? value,
        bool isOutOfSpec)
    {
        if (value is not null) Value = value;
        IsOutOfSpec = isOutOfSpec;
    }
}
