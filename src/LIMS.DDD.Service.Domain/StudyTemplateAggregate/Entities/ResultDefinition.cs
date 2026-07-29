using LIMS.DDD.Service.Domain.StudyTemplateAggregate.Ids;
using LIMS.DDD.Service.Domain.StudyTemplateAggregate.ValueObjects;

namespace LIMS.DDD.Service.Domain.StudyTemplateAggregate.Entities;

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

    internal void Update(
        string? resultInstance,
        string? unit,
        Specification? specification)
    {
        if (resultInstance is not null) ResultInstance = resultInstance;
        if (unit is not null) Unit = unit;
        if (specification is not null) Specification = specification;
    }

    public string ResultInstance { get; private set; } = string.Empty;

    public ResultDefinitionId Id { get; private set; }

    public StudyTemplateId StudyTemplateId { get; private set; }

    public string Unit { get; private set; } = string.Empty;

    public Specification Specification { get; private set; } = null!;
}
