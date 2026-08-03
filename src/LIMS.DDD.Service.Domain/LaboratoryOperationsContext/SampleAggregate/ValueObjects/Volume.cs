using LIMS.DDD.Service.Domain.SeedWork.Result;

namespace LIMS.DDD.Service.Domain.LaboratoryOperationsContext.SampleAggregate.ValueObjects;

public sealed record Volume
{
    public double? Value { get; }

    public string? Unit { get; }

    private Volume(
        double? value,
        string? unit)
    {
        Value = value;
        Unit = unit;
    }

    public static Result<Volume, Exception> Create(
        double? value,
        string? unit)
    {
        if (value is < 0) return Result<Volume, Exception>.Failure(new ArgumentException("Volume cannot be negative"));

        if (value.HasValue && string.IsNullOrWhiteSpace(unit))
            return Result<Volume, Exception>.Failure(
                new ArgumentException("Unit is required when volume is specified"));

        return Result<Volume, Exception>.Success(new Volume(value, unit));
    }
}
