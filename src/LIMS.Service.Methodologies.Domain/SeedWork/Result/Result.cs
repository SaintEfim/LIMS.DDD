namespace LIMS.Service.Methodologies.Domain.SeedWork.Result;

public readonly record struct None;

public class Result<TValue, TError>
    where TError : Exception
{
    private Result(
        bool isSuccess,
        TValue? value,
        TError? error)
    {
        IsSuccess = isSuccess;
        Value = value;
        Error = error;
    }

    private bool IsSuccess { get; }
    public bool IsFailure => !IsSuccess;
    private TValue? Value { get; }
    private TError? Error { get; }

    public static implicit operator Result<TValue, TError>(
        TValue value) =>
        new(true, value, null);

    public static implicit operator Result<TValue, TError>(
        TError error) =>
        new(false, default, error);

    public static Result<None, TError> Success()
    {
        return Result<None, TError>.Success(new None());
    }

    public static Result<TValue, TError> Success(
        TValue value)
    {
        return new Result<TValue, TError>(true, value, null);
    }

    public static Result<TValue, TError> Failure(
        TError error)
    {
        return new Result<TValue, TError>(false, default, error);
    }

    public TValue GetValue()
    {
        return IsFailure ? throw new InvalidOperationException("Cannot get value from failed result.") : Value!;
    }

    public TError GetError()
    {
        return IsFailure ? Error! : throw new InvalidOperationException("Cannot get error from failed result.");
    }

    public Result<TNew, TError> CastFailure<TNew>()
    {
        return Result<TNew, TError>.Failure(GetError());
    }
}
