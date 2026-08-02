using LIMS.DDD.Service.Domain.SeedWork.Result;

namespace LIMS.DDD.Service.Domain.StudyTemplateContext.StudyTemplateAggregate.ValueObjects;

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
            return Result<Revision, Exception>.Failure(new ArgumentException("Invalid revision.", nameof(value)));
        }

        if (value.Length > MaxRevisionLength)
        {
            return Result<Revision, Exception>.Failure(new ArgumentException(
                $"Revision length cannot exceed {MaxRevisionLength} characters. " + $"Current length: {value.Length}.",
                nameof(value)));
        }

        var revision = new Revision(value);
        return Result<Revision, Exception>.Success(revision);
    }
}
