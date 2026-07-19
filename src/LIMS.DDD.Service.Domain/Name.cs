namespace LIMS.DDD.Service.Domain;

public readonly record struct Name
{
    private const int MaxNameLength = 100;

    public string Value { get; }

    public Name(
        string value)
    {
        if (string.IsNullOrWhiteSpace(value))
        {
            throw new ArgumentException("Invalid name.", nameof(value));
        }

        if (value.Length > MaxNameLength)
        {
            throw new ArgumentException(
                $"Name length cannot exceed {MaxNameLength} characters. " + $"Current length: {value.Length}.",
                nameof(value));
        }

        Value = value;
    }
}
