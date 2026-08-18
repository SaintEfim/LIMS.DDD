using LIMS.Service.Methodologies.Domain.SeedWork;

namespace LIMS.Service.Methodologies.Domain.UnitSnapshots;

public readonly record struct UnitId(Guid Value) : IValueObjectId
{
    public Guid Value { get; } = Value;
}
