using Domain.SeedWork;

namespace LIMS.Service.LaboratoryOperations.Domain.SampleAggregate;

public readonly record struct SampleId(Guid Value) : IValueObjectId
{
    public Guid Value { get; } = Value;
}
