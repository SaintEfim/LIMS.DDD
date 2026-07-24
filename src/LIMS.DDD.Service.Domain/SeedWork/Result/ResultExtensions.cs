namespace LIMS.DDD.Service.Domain.SeedWork.Result;

public static class ResultExtensions
{
    public static async Task<Result<TNext, TError>> Bind<TValue, TNext, TError>(
        this Result<TValue, TError> result,
        Func<TValue, Task<Result<TNext, TError>>> func)
        where TError : Exception
    {
        if (result.IsFailure) return Result<TNext, TError>.Failure(result.Error!);

        return await func(result.Value!);
    }

    public static async Task<Result<TValue, TError>> OnSuccess<TValue, TError>(
        this Result<TValue, TError> result,
        Func<TValue, Task> action)
        where TError : Exception
    {
        if (result is { IsSuccess: true, Value: not null }) await action(result.Value);

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
}
