using LIMS.DDD.Service.Domain.SeedWork;

namespace LIMS.DDD.Service.Domain.StudyTemplateAggregate.StudyTemplateDeterminations;

public readonly record struct StudyTemplateDeterminationId(Guid Value) : IValueObjectId
{
    public Guid Value { get; } = Value;
}

public sealed class StudyTemplateDetermination
{
    private StudyTemplateDetermination()
    {
    }

    internal static StudyTemplateDetermination Create(
        StudyTemplateId studyTemplateId,
        string resultInstance,
        string unit,
        Specification specification)
    {
        var result = new StudyTemplateDetermination
        {
            Id = new StudyTemplateDeterminationId(Guid.NewGuid()),
            StudyTemplateId = studyTemplateId,
            ResultInstance = resultInstance,
            Unit = unit,
            Specification = specification
        };

        return result;
    }

    public string ResultInstance { get; private set; } = string.Empty;

    public StudyTemplateDeterminationId Id { get; private set; }

    public StudyTemplateId StudyTemplateId { get; private set; }

    public string Unit { get; private set; } = string.Empty;

    public Specification Specification { get; private set; } = null!;
}
