using LIMS.DDD.Service.Domain.SeedWork;

namespace LIMS.DDD.Service.Domain.StudyTemplateAggregate;

public readonly record struct StudyTemplateId(Guid Value) : IValueObjectId
{
    public Guid Value { get; } = Value;
}
