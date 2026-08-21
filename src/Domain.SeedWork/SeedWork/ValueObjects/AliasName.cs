using Domain.SeedWork.SeedWork.Result;

namespace Domain.SeedWork.SeedWork.ValueObjects;

public sealed record AliasName
{
    private const int MaxAliasNameLength = 100;

    // for EF Core
    private AliasName()
    {
    }

    private AliasName(
        string value)
    {
        Value = value;
    }

    public string Value { get; } = null!;

    public static Result<AliasName, Exception> Create(
        string value)
    {
        if (string.IsNullOrWhiteSpace(value))
        {
            return new ArgumentException("Invalid name.", nameof(value));
        }

        if (value.Length > MaxAliasNameLength)
        {
            return new ArgumentException(
                $"AliasName length cannot exceed {MaxAliasNameLength} characters. " +
                $"Current length: {value.Length}.", nameof(value));
        }

        var aliasName = new AliasName(value);
        return aliasName;
    }
}
