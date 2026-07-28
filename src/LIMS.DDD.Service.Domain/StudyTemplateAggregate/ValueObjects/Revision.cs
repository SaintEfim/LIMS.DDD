using LIMS.DDD.Service.Domain.SeedWork.Result;

namespace LIMS.DDD.Service.Domain.StudyTemplateAggregate.ValueObjects;

public readonly record struct Revision
{
    private const int MaxRevisionLength = 100;

    private Revision(
        string value) =>
        Value = value;

    public string Value { get; }

    public static Result<Revision, Exception> Create(
        string value)
    {
        if (string.IsNullOrWhiteSpace(value))
        {
            throw new ArgumentException("Invalid name.", nameof(value));
        }

        if (value.Length > MaxRevisionLength)
        {
            throw new ArgumentException(
                $"Revision length cannot exceed {MaxRevisionLength} characters. " + $"Current length: {value.Length}.",
                nameof(value));
        }

        var revision = new Revision(value);
        return Result<Revision, Exception>.Success(revision);
    }
}
