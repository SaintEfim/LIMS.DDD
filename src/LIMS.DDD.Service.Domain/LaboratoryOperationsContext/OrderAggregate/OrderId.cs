using LIMS.DDD.Service.Domain.SeedWork;

namespace LIMS.DDD.Service.Domain.LaboratoryOperationsContext.OrderAggregate;

public readonly record struct OrderId(Guid Value) : IValueObjectId
{
    public Guid Value { get; } = Value;
}
