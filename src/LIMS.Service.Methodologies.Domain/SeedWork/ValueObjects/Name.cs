using LIMS.Service.Methodologies.Domain.SeedWork.Result;

namespace LIMS.Service.Methodologies.Domain.SeedWork.ValueObjects;

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
            return Result<Name, Exception>.Failure(new ArgumentException("Invalid name.", nameof(value)));
        }

        if (value.Length > MaxNameLength)
        {
            return Result<Name, Exception>.Failure(new ArgumentException(
                $"Name length cannot exceed {MaxNameLength} characters. " + $"Current length: {value.Length}.",
                nameof(value)));
        }

        var name = new Name(value);
        return Result<Name, Exception>.Success(name);
    }
}
