using Domain.SeedWork.Errors;
using Domain.SeedWork.Result;
using LIMS.Service.LaboratoryOperations.Domain.UnitSnapshots;

namespace LIMS.Service.LaboratoryOperations.Domain.SampleAggregate.ValueObjects;

public sealed record Volume
{
    private Volume(
        double? value,
        UnitId? unitId)
    {
        Value = value;
        UnitId = unitId;
    }

    // for EF Core
    private Volume()
    {
    }

    public double? Value { get; private set; }
    public UnitId? UnitId { get; private set; }

    public static Result<Volume, DomainError> Create(
        double? value,
        UnitId? unitId)
    {
        if (value is < 0)
        {
            return new ValidationError("Volume cannot be negative.");
        }

        if (value.HasValue && unitId is null)
        {
            return new ValidationError("Unit is required when volume is specified.");
        }

        return new Volume(value, unitId);
    }

    internal void Update(
        double? value,
        UnitId? unitId)
    {
        if (value.HasValue)
        {
            Value = value;
        }

        if (unitId is not null)
        {
            UnitId = unitId;
        }
    }
}
