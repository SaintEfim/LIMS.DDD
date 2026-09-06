using Library.Domain.SeedWork;
using Library.Domain.SeedWork.ValueObjects;
using LIMS.Service.LaboratoryOperations.Domain.UnitSnapshots;

namespace LIMS.Service.LaboratoryOperations.Domain.StudyTemplateSnapshots.ResultDefinitions;

public sealed class ResultDefinitionSnapshot : ISoftDeletable
{
    private ResultDefinitionSnapshot()
    {
    }

    public ResultDefinitionSnapshot(
        ResultDefinitionId id,
        StudyTemplateId studyTemplateId,
        string resultInstance,
        UnitId unitId,
        Specification specification)
    {
        Id = id;
        StudyTemplateId = studyTemplateId;
        ResultInstance = resultInstance;
        UnitId = unitId;
        Specification = specification;
    }

    public ResultDefinitionId Id { get; private set; }
    public StudyTemplateId StudyTemplateId { get; private set; }
    public string ResultInstance { get; private set; } = string.Empty;
    public UnitId UnitId { get; private set; }
    public Specification Specification { get; private set; } = null!;
    public bool IsDeleted { get; private set; }
    public DateTimeOffset? DeletedAt { get; private set; }
}
