using Domain.SeedWork.SeedWork.Result;

namespace Domain.SeedWork.SeedWork.ValueObjects;

public sealed record Description
{
    private const int MaxDescriptionLength = 1000;

    // for EF Core
    private Description()
    {
    }

    private Description(
        string? value)
    {
        Value = value;
    }

    public string? Value { get; }

    public static Result<Description, Exception> Create(
        string? value)
    {
        var descriptionValue = string.IsNullOrWhiteSpace(value) ? string.Empty : value.Trim();

        if (descriptionValue.Length > MaxDescriptionLength)
        {
            return new ArgumentException(
                $"Description length cannot exceed {MaxDescriptionLength} characters. " +
                $"Current length: {descriptionValue.Length}.", nameof(value));
        }

        var description = new Description(descriptionValue);
        return description;
    }
}
