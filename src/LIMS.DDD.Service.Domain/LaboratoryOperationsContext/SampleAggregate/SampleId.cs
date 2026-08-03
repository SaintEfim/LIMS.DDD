using LIMS.DDD.Service.Domain.SeedWork;

namespace LIMS.DDD.Service.Domain.LaboratoryOperationsContext.SampleAggregate;

public readonly record struct SampleId(Guid Value) : IValueObjectId
{
    public Guid Value { get; } = Value;
}
