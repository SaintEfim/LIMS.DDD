using LIMS.DDD.Service.Domain.SeedWork;

namespace LIMS.DDD.Service.Domain.StudyAggregate.Ids;

public readonly record struct StudyId(Guid Value) : IValueObjectId
{
    public Guid Value { get; } = Value;
}
