using LIMS.Service.LaboratoryOperations.Domain.SeedWork;

namespace LIMS.Service.LaboratoryOperations.Domain.UnitSnapshots;

public readonly record struct UnitId(Guid Value) : IValueObjectId
{
    public Guid Value { get; } = Value;
}
