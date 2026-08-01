using LIMS.DDD.Service.Domain.SeedWork;

namespace LIMS.DDD.Service.Domain.LaboratoryOperationsContext.Ids;

public readonly record struct TestId(Guid Value) : IValueObjectId
{
    public Guid Value { get; } = Value;
}
