using Domain.SeedWork.SeedWork;

namespace LIMS.Service.LaboratoryOperations.Domain.OrderAggregate;

public readonly record struct OrderId(Guid Value) : IValueObjectId
{
    public Guid Value { get; } = Value;
}
