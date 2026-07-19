namespace LIMS.DDD.Service.Domain.StudyTemplateAggregate.Result;

public sealed class StudyTemplateResult
{
    private StudyTemplateResult()
    {
    }

    internal StudyTemplateResult(
        StudyTemplateId studyTemplateId,
        string unit,
        ValueRange valueRange)
    {
        Id = new StudyTemplateResultId(Guid.NewGuid());
        StudyTemplateId = studyTemplateId;
        Unit = unit;
        ValueRange = valueRange;
    }

    public StudyTemplateResultId Id { get; private set; }

    public StudyTemplateId StudyTemplateId { get; private set; }

    public string Unit { get; private set; } = string.Empty;

    public ValueRange ValueRange { get; private set; } = null!;
}
