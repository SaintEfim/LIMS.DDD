namespace LIMS.DDD.Service.Domain.SeedWork.Result;

public readonly record struct UnitEmpty;

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

    public bool IsSuccess { get; }
    public bool IsFailure => !IsSuccess;
    private TValue? Value { get; }
    private TError? Error { get; }

    public static Result<TValue, TError> Success(
        TValue? value)
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
        return IsFailure ? throw new InvalidOperationException("Cannot get error from failed result.") : Error!;
    }

    public Result<TNew, TError> CastFailure<TNew>()
    {
        return Result<TNew, TError>.Failure(GetError());
    }
}
