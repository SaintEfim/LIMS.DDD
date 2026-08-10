using LIMS.DDD.Service.Domain.SeedWork;
using LIMS.DDD.Service.Domain.SeedWork.ValueObjects;

namespace LIMS.DDD.Service.Domain.LaboratoryOperationsContext.StudyAggregate.ValueObjects;

public readonly record struct TemplateId(Guid Value) : IValueObjectId
{
    public Guid Value { get; } = Value;
}

public readonly record struct ParameterTemplateId(Guid Value) : IValueObjectId
{
    public Guid Value { get; } = Value;
}

public readonly record struct ResultTemplateId(Guid Value) : IValueObjectId
{
    public Guid Value { get; } = Value;
}

public sealed record StudyTemplateCreateSnapshot(
    TemplateId TemplateId,
    Name Name,
    bool CanCreateStudy,
    IReadOnlyList<ParameterSnapshot> Parameters,
    IReadOnlyList<ResultSnapshot> Results);

public sealed record ParameterSnapshot
{
    private ParameterSnapshot()
    {
    }

    public ParameterSnapshot(
        ParameterTemplateId inputParameterId,
        Name name,
        AliasName aliasName,
        Specification specification)
    {
        InputParameterId = inputParameterId;
        Name = name;
        AliasName = aliasName;
        Specification = specification;
    }

    public ParameterTemplateId InputParameterId { get; private set; }

    public Name Name { get; private set; }

    public AliasName AliasName { get; private set; }

    public Specification Specification { get; private set; }
}

public sealed record ResultSnapshot
{
    private ResultSnapshot()
    {
    }

    public ResultSnapshot(
        ResultTemplateId resultDefinitionId,
        string resultInstance,
        string unit,
        Specification specification)
    {
        ResultDefinitionId = resultDefinitionId;
        ResultInstance = resultInstance;
        Unit = unit;
        Specification = specification;
    }

    public ResultTemplateId ResultDefinitionId { get; private set; }

    public string ResultInstance { get; private set; }

    public string Unit { get; private set; }

    public Specification Specification { get; private set; }
}
