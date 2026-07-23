namespace LIMS.DDD.Service.Domain.SeedWork.Result;

public static class ResultExtensions
{
    public static Result<TValue, TError> OnSuccess<TValue, TError>(
        this Result<TValue, TError> result,
        Action<TValue> action)
        where TError : Exception
    {
        if (result is { IsSuccess: true, Value: not null }) action(result.Value);

        return result;
    }

    public static async Task<Result<TValue, TError>> OnSuccess<TValue, TError>(
        this Result<TValue, TError> result,
        Func<TValue, Task> action)
        where TError : Exception
    {
        if (result is { IsSuccess: true, Value: not null }) await action(result.Value);

        return result;
    }

    public static Result<TValue, TError> OnFailure<TValue, TError>(
        this Result<TValue, TError> result,
        Action<TError> action)
        where TError : Exception
    {
        if (result is { IsFailure: true, Error: not null }) action(result.Error);

        return result;
    }

    public static async Task<Result<TValue, TError>> OnFailure<TValue, TError>(
        this Result<TValue, TError> result,
        Func<TError, Task> action)
        where TError : Exception
    {
        if (result is { IsFailure: true, Error: not null }) await action(result.Error);

        return result;
    }

    public static Result<TNewValue, TError> Map<TValue, TError, TNewValue>(
        this Result<TValue, TError> result,
        Func<TValue, TNewValue> transform)
        where TError : Exception
    {
        return result.IsFailure
            ? Result<TNewValue, TError>.Failure(result.Error!)
            : Result<TNewValue, TError>.Success(transform(result.Value!));
    }

    public static async Task<Result<TNewValue, TError>> Map<TValue, TError, TNewValue>(
        this Result<TValue, TError> result,
        Func<TValue, Task<TNewValue>> transform)
        where TError : Exception
    {
        if (result.IsFailure) return Result<TNewValue, TError>.Failure(result.Error!);

        var newValue = await transform(result.Value!);
        return Result<TNewValue, TError>.Success(newValue);
    }

    public static Result<TError> OnSuccess<TError>(
        this Result<TError> result,
        Action action)
        where TError : Exception
    {
        if (result.IsSuccess)
            action();

        return result;
    }

    public static async Task<Result<TError>> OnSuccess<TError>(
        this Result<TError> result,
        Func<Task> action)
        where TError : Exception
    {
        if (result.IsSuccess)
            await action();

        return result;
    }

    public static Result<TError> OnFailure<TError>(
        this Result<TError> result,
        Action<TError> action)
        where TError : Exception
    {
        if (result is { IsFailure: true, Error: not null })
            action(result.Error);

        return result;
    }
}
