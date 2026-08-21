using Domain.SeedWork.Result;

namespace Domain.SeedWork.ValueObjects;

public sealed record Specification
{
    private Specification(
        double? minValue,
        double? maxValue)
    {
        MinValue = minValue;
        MaxValue = maxValue;
    }

    // for EF Core
    private Specification()
    {
    }

    public double? MinValue { get; init; }

    public double? MaxValue { get; init; }

    public static Result<Specification, Exception> Create(
        double? minValue,
        double? maxValue)
    {
        if (minValue.HasValue && maxValue.HasValue && minValue.Value > maxValue.Value)
        {
            return new ArgumentException($"Min value ({minValue}) cannot be greater than max value ({maxValue}).");
        }

        var specification = new Specification(minValue, maxValue);
        return specification;
    }

    public bool IsWithinSpec(
        double value)
    {
        if (value < MinValue)
        {
            return false;
        }

        return !MaxValue.HasValue || !(value > MaxValue.Value);
    }
}
