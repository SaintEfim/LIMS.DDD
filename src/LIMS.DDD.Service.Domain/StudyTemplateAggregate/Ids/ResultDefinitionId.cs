using LIMS.DDD.Service.Domain.SeedWork;

namespace LIMS.DDD.Service.Domain.StudyTemplateAggregate.Ids;

public readonly record struct ResultDefinitionId(Guid Value) : IValueObjectId
{
    public Guid Value { get; } = Value;
}
