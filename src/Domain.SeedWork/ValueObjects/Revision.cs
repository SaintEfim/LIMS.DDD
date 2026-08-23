using Domain.SeedWork.Errors;
using Domain.SeedWork.Result;

namespace Domain.SeedWork.ValueObjects;

public sealed record Revision
{
    private const int MaxRevisionLength = 100;

    // for EF Core
    private Revision()
    {
    }

    private Revision(
        string value)
    {
        Value = value;
    }

    public string Value { get; } = null!;

    public static Result<Revision, DomainError> Create(
        string value)
    {
        if (string.IsNullOrWhiteSpace(value))
        {
            return new ValidationError("Revision cannot be empty.");
        }

        if (value.Length > MaxRevisionLength)
        {
            return new ValidationError(
                $"Revision length cannot exceed {MaxRevisionLength} characters. Current length: {value.Length}.");
        }

        var revision = new Revision(value.Trim());
        return revision;
    }
}
