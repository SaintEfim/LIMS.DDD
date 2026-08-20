using Domain.SeedWork.SeedWork.Result;

namespace Domain.SeedWork.SeedWork.ValueObjects;

public sealed record Name
{
    private const int MaxNameLength = 100;

    // for EF Core
    private Name() { }

    private Name(
        string value)
    {
        Value = value;
    }

    public string Value { get; }

    public static Result<Name, Exception> Create(
        string value)
    {
        if (string.IsNullOrWhiteSpace(value))
        {
            return new ArgumentException("Invalid name.", nameof(value));
        }

        if (value.Length > MaxNameLength)
        {
            return new ArgumentException(
                $"Name length cannot exceed {MaxNameLength} characters. " + $"Current length: {value.Length}.",
                nameof(value));
        }

        var name = new Name(value);
        return name;
    }
}
