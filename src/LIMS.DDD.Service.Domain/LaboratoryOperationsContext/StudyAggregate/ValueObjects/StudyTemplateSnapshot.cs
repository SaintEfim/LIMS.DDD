namespace LIMS.DDD.Service.Domain.LaboratoryOperationsContext.StudyAggregate.ValueObjects;

public sealed record StudyTemplateSnapshot(
    Guid TemplateId,
    string Name,
    IReadOnlyList<ParameterSnapshot> Parameters,
    IReadOnlyList<ResultSnapshot> Results);

public sealed record ParameterSnapshot(
    Guid InputParameterId,
    string Name,
    string AliasName,
    double? MinValue,
    double? MaxValue);

public sealed record ResultSnapshot(
    Guid ResultDefinitionId,
    string ResultInstance,
    string Unit,
    double? MinValue,
    double? MaxValue);
