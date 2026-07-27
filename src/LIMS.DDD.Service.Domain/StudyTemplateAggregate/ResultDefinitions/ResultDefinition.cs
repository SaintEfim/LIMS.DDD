using LIMS.DDD.Service.Domain.SeedWork;

namespace LIMS.DDD.Service.Domain.StudyTemplateAggregate.ResultDefinitions;

public readonly record struct ResultDefinitionId(Guid Value) : IValueObjectId
{
    public Guid Value { get; } = Value;
}

public sealed class ResultDefinition
{
    private ResultDefinition()
    {
    }

    internal static ResultDefinition Create(
        StudyTemplateId studyTemplateId,
        string resultInstance,
        string unit,
        Specification specification)
    {
        var result = new ResultDefinition
        {
            Id = new ResultDefinitionId(Guid.NewGuid()),
            StudyTemplateId = studyTemplateId,
            ResultInstance = resultInstance,
            Unit = unit,
            Specification = specification
        };

        return result;
    }

    public string ResultInstance { get; private set; } = string.Empty;

    public ResultDefinitionId Id { get; private set; }

    public StudyTemplateId StudyTemplateId { get; private set; }

    public string Unit { get; private set; } = string.Empty;

    public Specification Specification { get; private set; } = null!;
}
