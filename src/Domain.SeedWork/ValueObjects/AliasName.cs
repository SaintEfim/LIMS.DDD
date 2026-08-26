using Domain.SeedWork.Errors;
using Domain.SeedWork.Result;

namespace Domain.SeedWork.ValueObjects;

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

    public static Result<AliasName, DomainError> Create(
        string value)
    {
        if (string.IsNullOrWhiteSpace(value))
        {
            return new ValidationError("Alias name cannot be empty.");
        }

        if (value.Length > MaxAliasNameLength)
        {
            return new ValidationError($"Alias name length cannot exceed {MaxAliasNameLength} characters. " +
                                       $"Current length: {value.Length}.");
        }

        var aliasName = new AliasName(value.Trim());
        return aliasName;
    }
}
