using Domain.SeedWork.SeedWork.Result;

namespace LIMS.Service.LaboratoryOperations.Domain.ValueObjects;

public sealed record Code
{
    private const int MaxCodeLength = 100;

    private Code(
        string? value)
    {
        Value = value;
    }

    public string? Value { get; }

    public static Result<Code, Exception> Create(
        string? code)
    {
        if (code is not null && code.Length > MaxCodeLength)
        {
            return Result<Code, Exception>.Failure(
                new ArgumentException($"Code cannot exceed {MaxCodeLength} characters"));
        }

        return Result<Code, Exception>.Success(new Code(code?.Trim()));
    }
}
