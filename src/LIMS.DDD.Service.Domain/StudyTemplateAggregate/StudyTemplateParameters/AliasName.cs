namespace LIMS.DDD.Service.Domain.StudyTemplateAggregate.StudyTemplateParameters;

public readonly record struct AliasName
{
    private const int MaxAliasNameLength = 100;

    public string Value { get; }

    public AliasName(
        string value)
    {
        if (string.IsNullOrWhiteSpace(value))
        {
            throw new ArgumentException("Invalid name.", nameof(value));
        }

        if (value.Length > MaxAliasNameLength)
        {
            throw new ArgumentException(
                $"AliasName length cannot exceed {MaxAliasNameLength} characters. " +
                $"Current length: {value.Length}.", nameof(value));
        }

        Value = value;
    }
}
