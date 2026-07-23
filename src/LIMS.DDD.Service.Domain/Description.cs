namespace LIMS.DDD.Service.Domain;

public readonly record struct Description
{
    private const int MaxDescriptionLength = 1000;

    public string Value { get; }

    public Description(
        string? value)
    {
        Value = string.IsNullOrWhiteSpace(value) ? string.Empty : value.Trim();

        if (Value.Length > MaxDescriptionLength)
        {
            throw new ArgumentException(
                $"Description length cannot exceed {MaxDescriptionLength} characters. " +
                $"Current length: {Value.Length}.", nameof(value));
        }
    }
}
