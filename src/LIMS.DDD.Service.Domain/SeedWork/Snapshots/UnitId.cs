namespace LIMS.DDD.Service.Domain.SeedWork.Snapshots;

public readonly record struct UnitId(Guid Value) : IValueObjectId
{
    public Guid Value { get; } = Value;
}
