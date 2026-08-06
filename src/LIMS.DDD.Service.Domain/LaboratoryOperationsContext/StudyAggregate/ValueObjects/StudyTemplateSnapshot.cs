using LIMS.DDD.Service.Domain.SeedWork;
using LIMS.DDD.Service.Domain.SeedWork.ValueObjects;

namespace LIMS.DDD.Service.Domain.LaboratoryOperationsContext.StudyAggregate.ValueObjects;

public readonly record struct TemplateId(Guid Value) : IValueObjectId
{
    public Guid Value { get; } = Value;
}

public sealed record StudyTemplateCreateSnapshot(
    TemplateId TemplateId,
    Name Name,
    IReadOnlyList<ParameterSnapshot> Parameters,
    IReadOnlyList<ResultSnapshot> Results);

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
