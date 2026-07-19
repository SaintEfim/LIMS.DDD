namespace LIMS.DDD.Service.Domain.StudyTemplate.Result;

public sealed class StudyTemplateResult
{
    private StudyTemplateResult()
    {
    }

    public StudyTemplateResultId Id { get; private set; }

    public StudyTemplateId StudyTemplateId { get; private set; }

    public string Unit { get; private set; } = string.Empty;

    public ValueRange? ValueRange { get; private set; }

    public static StudyTemplateResult Create(
        StudyTemplateId studyTemplateId,
        string unit,
        ValueRange? valueRange)
    {
        var result = new StudyTemplateResult
        {
            Id = new StudyTemplateResultId(Guid.NewGuid()),
            StudyTemplateId = studyTemplateId,
            Unit = unit,
            ValueRange = valueRange
        };

        return result;
    }
}
