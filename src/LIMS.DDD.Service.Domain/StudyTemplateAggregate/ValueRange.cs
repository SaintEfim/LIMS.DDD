namespace LIMS.DDD.Service.Domain.StudyTemplateAggregate;

public record ValueRange(double? MinValue, double? MaxValue)
{
    public static ValueRange Create(
        double? minValue,
        double? maxValue)
    {
        if (minValue.HasValue && maxValue.HasValue && minValue.Value > maxValue.Value)
        {
            throw new ArgumentException($"Min value ({minValue}) cannot be greater than max value ({maxValue}).");
        }

        return new ValueRange(minValue, maxValue);
    }

    public bool Contains(
        double value)
    {
        if (value < MinValue) return false;
        return !(value > MaxValue);
    }
}
