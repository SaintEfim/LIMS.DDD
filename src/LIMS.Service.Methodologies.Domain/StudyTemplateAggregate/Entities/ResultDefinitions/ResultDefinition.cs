using LIMS.Service.Methodologies.Domain.SeedWork;
using LIMS.Service.Methodologies.Domain.SeedWork.ValueObjects;
using LIMS.Service.Methodologies.Domain.UnitSnapshots;

namespace LIMS.Service.Methodologies.Domain.StudyTemplateAggregate.Entities.ResultDefinitions;

public sealed class ResultDefinition : SoftDeletableModel
{
    public string ResultInstance { get; private set; }

    public ResultDefinitionId Id { get; private set; }

    public StudyTemplateId StudyTemplateId { get; private set; }

    public UnitId UnitId { get; private set; }

    public Specification Specification { get; private set; }

    internal ResultDefinition(
        StudyTemplateId studyTemplateId,
        string resultInstance,
        UnitId unitId,
        Specification specification)
    {
        Id = new ResultDefinitionId(Guid.NewGuid());
        StudyTemplateId = studyTemplateId;
        ResultInstance = resultInstance;
        UnitId = unitId;
        Specification = specification;
    }

    internal void Update(
        string? resultInstance,
        UnitId? unitId,
        Specification? specification)
    {
        if (resultInstance is not null)
        {
            ResultInstance = resultInstance;
        }

        if (unitId is not null)
        {
            UnitId = unitId.Value;
        }

        if (specification is not null)
        {
            Specification = specification;
        }
    }

    internal void MarkAsDeleted()
    {
        IsDeleted = true;
        DeletedAt = DateTimeOffset.UtcNow;
    }
}
