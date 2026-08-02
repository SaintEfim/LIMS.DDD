using LIMS.DDD.Service.Domain.SeedWork.Result;

namespace LIMS.DDD.Service.Domain.StudyTemplateContext.StudyTemplateAggregate.ValueObjects;

public sealed record Specification
{
    public double? MinValue { get; init; }

    public double? MaxValue { get; init; }

    private Specification(
        double? minValue,
        double? maxValue)
    {
        MinValue = minValue;
        MaxValue = maxValue;
    }

    public static Result<Specification, Exception> Create(
        double? minValue,
        double? maxValue)
    {
        if (minValue.HasValue && maxValue.HasValue && minValue.Value > maxValue.Value)
        {
            return Result<Specification, Exception>.Failure(
                new ArgumentException($"Min value ({minValue}) cannot be greater than max value ({maxValue})."));
        }

        var specification = new Specification(minValue, maxValue);
        return Result<Specification, Exception>.Success(specification);
    }

    public bool Contains(
        double value)
    {
        if (value < MinValue) return false;
        return !(value > MaxValue);
    }
}
