using LIMS.DDD.Service.Domain.SeedWork.Result;

namespace LIMS.DDD.Service.Domain.LaboratoryOperationsContext.ValueObjects;

public readonly record struct Code
{
    public string? Value { get; }

    private Code(
        string? value)
    {
        Value = value;
    }

    public static Result<Code, Exception> Create(
        string? code)
    {
        if (code is not null && code.Length > 100)
            return Result<Code, Exception>.Failure(new ArgumentException("Code cannot exceed 100 characters"));

        return Result<Code, Exception>.Success(new Code(code?.Trim()));
    }
}
