namespace LIMS.DDD.Service.Domain.StudyTemplateAggregate;

public readonly record struct Revision
{
    private const int MaxRevisionLength = 100;

    public string Value { get; }

    public Revision(
        string value)
    {
        if (string.IsNullOrWhiteSpace(value))
        {
            throw new ArgumentException("Invalid name.", nameof(value));
        }

        if (value.Length > MaxRevisionLength)
        {
            throw new ArgumentException(
                $"Revision length cannot exceed {MaxRevisionLength} characters. " + $"Current length: {value.Length}.",
                nameof(value));
        }

        Value = value;
    }
}
