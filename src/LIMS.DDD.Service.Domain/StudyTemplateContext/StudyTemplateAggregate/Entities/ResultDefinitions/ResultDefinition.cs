using LIMS.DDD.Service.Domain.SeedWork;
using LIMS.DDD.Service.Domain.StudyTemplateContext.StudyTemplateAggregate.ValueObjects;

namespace LIMS.DDD.Service.Domain.StudyTemplateContext.StudyTemplateAggregate.Entities.ResultDefinitions;

public sealed class ResultDefinition : SoftDeletableModel
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

    internal void MarkAsDeleted()
    {
        IsDeleted = true;
        DeletedAt = DateTimeOffset.UtcNow;
    }

    public string ResultInstance { get; private set; } = string.Empty;

    public ResultDefinitionId Id { get; private set; }

    public StudyTemplateId StudyTemplateId { get; private set; }

    public string Unit { get; private set; } = string.Empty;

    public Specification Specification { get; private set; } = null!;
}
