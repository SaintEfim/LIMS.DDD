using LIMS.DDD.Service.Domain.SeedWork.Result;

namespace LIMS.DDD.Service.Domain.StudyTemplateAggregate.InputParameters;

public readonly record struct AliasName
{
    private const int MaxAliasNameLength = 100;

    public string Value { get; }

    private AliasName(
        string value) =>
        Value = value;

    public static Result<AliasName, Exception> Create(
        string value)
    {
        if (string.IsNullOrWhiteSpace(value))
        {
            return Result<AliasName, Exception>.Failure(new ArgumentException("Invalid name.", nameof(value)));
        }

        if (value.Length > MaxAliasNameLength)
        {
            return Result<AliasName, Exception>.Failure(new ArgumentException(
                $"AliasName length cannot exceed {MaxAliasNameLength} characters. " +
                $"Current length: {value.Length}.", nameof(value)));
        }

        var aliasName = new AliasName(value);
        return Result<AliasName, Exception>.Success(aliasName);
    }
}
