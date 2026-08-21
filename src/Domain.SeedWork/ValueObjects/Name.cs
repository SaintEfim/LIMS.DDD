using Domain.SeedWork.Errors;
using Domain.SeedWork.Result;

namespace Domain.SeedWork.ValueObjects;

public sealed record Name
{
    private const int MaxNameLength = 100;

    // for EF Core
    private Name()
    {
    }

    private Name(
        string value)
    {
        Value = value;
    }

    public string Value { get; } = null!;

    public static Result<Name, Exception> Create(
        string value)
    {
        if (string.IsNullOrWhiteSpace(value))
        {
            return new ValidationException("Name cannot be empty.");
        }

        if (value.Length > MaxNameLength)
        {
            return new ValidationException(
                $"Name length cannot exceed {MaxNameLength} characters. Current length: {value.Length}.");
        }

        var name = new Name(value.Trim());
        return name;
    }
}
