using Domain.SeedWork;

namespace LIMS.Service.LaboratoryOperations.Domain.StudyAggregate.Entities;

public readonly record struct MeasuredValueId(Guid Value) : IValueObjectId
{
    public Guid Value { get; } = Value;
}
