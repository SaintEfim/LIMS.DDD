namespace LIMS.DDD.Service.Domain;

public readonly record struct Description
{
    private const int MaxDescriptionLength = 1000;

    public string Value { get; }

    public Description(
        string value)
    {
        if (string.IsNullOrWhiteSpace(value))
        {
            throw new ArgumentException("Invalid name.", nameof(value));
        }

        if (value.Length > MaxDescriptionLength)
        {
            throw new ArgumentException(
                $"Description length cannot exceed {MaxDescriptionLength} characters. " +
                $"Current length: {value.Length}.", nameof(value));
        }

        Value = value;
    }
}
