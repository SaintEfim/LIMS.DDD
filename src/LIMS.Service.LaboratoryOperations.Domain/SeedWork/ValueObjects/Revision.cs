using LIMS.Service.LaboratoryOperations.Domain.SeedWork.Result;

namespace LIMS.Service.LaboratoryOperations.Domain.SeedWork.ValueObjects;

public sealed record Revision
{
    private const int MaxRevisionLength = 100;

    private Revision(
        string value)
    {
        Value = value;
    }

    public string Value { get; }

    public static Result<Revision, Exception> Create(
        string value)
    {
        if (string.IsNullOrWhiteSpace(value))
        {
            return new ArgumentException("Invalid revision.", nameof(value));
        }

        if (value.Length > MaxRevisionLength)
        {
            return new ArgumentException(
                $"Revision length cannot exceed {MaxRevisionLength} characters. " + $"Current length: {value.Length}.",
                nameof(value));
        }

        var revision = new Revision(value);
        return revision;
    }
}
