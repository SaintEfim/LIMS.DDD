namespace LIMS.DDD.Service.Domain.StudyTemplate;

public readonly record struct ValueRange
{
    public double? Min { get; }
    public double? Max { get; }

    public ValueRange(
        double? min,
        double? max)
    {
        if (min.HasValue && max.HasValue && min.Value > max.Value)
        {
            throw new ArgumentException($"Min value ({min}) cannot be greater than max value ({max}).");
        }

        Min = min;
        Max = max;
    }

    public bool Contains(
        double value)
    {
        if (value < Min) return false;
        return !(value > Max);
    }
}
