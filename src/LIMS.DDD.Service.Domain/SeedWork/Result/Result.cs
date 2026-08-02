namespace LIMS.DDD.Service.Domain.SeedWork.Result;

public class Result<TError>
    where TError : Exception
{
    public bool IsSuccess { get; }
    public bool IsFailure => !IsSuccess;
    public TError? Error { get; }

    private Result(
        bool isSuccess,
        TError? error)
    {
        IsSuccess = isSuccess;
        Error = error;
    }

    public static Result<TError> Success() => new(true, null);

    public static Result<TError> Failure(
        TError error) =>
        new(false, error);
}

public class Result<TValue, TError>
    where TError : Exception
{
    public bool IsSuccess { get; }
    public bool IsFailure => !IsSuccess;
    private TValue? Value { get; }
    public TError? Error { get; }

    private Result(
        bool isSuccess,
        TValue? value,
        TError? error)
    {
        IsSuccess = isSuccess;
        Value = value;
        Error = error;
    }

    public static Result<TValue, TError> Success(
        TValue? value) =>
        new(true, value, null);

    public static Result<TValue, TError> Failure(
        TError error) =>
        new(false, default, error);

    public TValue GetValue()
    {
        return IsFailure ? throw new InvalidOperationException("Cannot get value from failed result.") : Value!;
    }
}
