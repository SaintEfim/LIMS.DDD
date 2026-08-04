using LIMS.DDD.Service.Domain.SeedWork;

namespace LIMS.DDD.Service.Domain.LaboratoryOperationsContext.StudyAggregate;

public readonly record struct StudyId(Guid Value) : IValueObjectId
{
    public Guid Value { get; } = Value;
}
