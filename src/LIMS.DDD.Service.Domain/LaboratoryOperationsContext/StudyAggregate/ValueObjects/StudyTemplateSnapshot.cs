using LIMS.DDD.Service.Domain.SeedWork.ValueObjects;

namespace LIMS.DDD.Service.Domain.LaboratoryOperationsContext.StudyAggregate.ValueObjects;

public sealed record StudyTemplateCreateSnapshot(
    Guid TemplateId,
    Name Name,
    IReadOnlyList<ParameterSnapshot> Parameters,
    IReadOnlyList<ResultSnapshot> Results)
{
    public StudyTemplateSnapshot ToStudySnapshot()
    {
        return new StudyTemplateSnapshot(TemplateId, Name);
    }
}

public sealed record StudyTemplateSnapshot(Guid TemplateId, Name Name);

public sealed record ParameterSnapshot(
    Guid InputParameterId,
    Name Name,
    AliasName AliasName,
    double? MinValue,
    double? MaxValue);

public sealed record ResultSnapshot(
    Guid ResultDefinitionId,
    string ResultInstance,
    string Unit,
    double? MinValue,
    double? MaxValue);
