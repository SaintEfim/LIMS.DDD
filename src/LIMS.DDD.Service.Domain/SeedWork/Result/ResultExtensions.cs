namespace LIMS.DDD.Service.Domain.SeedWork.Result;

public static class ResultExtensions
{
    // 1. Sync Result + Sync Map (T -> TNext) -> Sync Result
    public static Result<TNext, TError> Map<TValue, TNext, TError>(
        this Result<TValue, TError> result,
        Func<TValue, TNext> func)
        where TError : Exception
    {
        return result.IsFailure
            ? Result<TNext, TError>.Failure(result.Error!)
            : Result<TNext, TError>.Success(func(result.Value!));
    }

    // 2. Sync Result + Sync Bind (T -> Result) -> Sync Result
    public static Result<TNext, TError> Bind<TValue, TNext, TError>(
        this Result<TValue, TError> result,
        Func<TValue, Result<TNext, TError>> func)
        where TError : Exception
    {
        return result.IsFailure ? Result<TNext, TError>.Failure(result.Error!) : func(result.Value!);
    }

    // 3. Sync Result + Async Bind (T -> Task<Result>) -> Task<Result>
    public static async Task<Result<TNext, TError>> Bind<TValue, TNext, TError>(
        this Result<TValue, TError> result,
        Func<TValue, Task<Result<TNext, TError>>> func)
        where TError : Exception
    {
        if (result.IsFailure) return Result<TNext, TError>.Failure(result.Error!);
        return await func(result.Value!);
    }

    // 4. Task<Result> + Sync Map (T -> TNext) -> Task<Result>
    public static async Task<Result<TNext, TError>> Map<TValue, TNext, TError>(
        this Task<Result<TValue, TError>> resultTask,
        Func<TValue, TNext> func)
        where TError : Exception
    {
        var result = await resultTask;
        return result.IsFailure
            ? Result<TNext, TError>.Failure(result.Error!)
            : Result<TNext, TError>.Success(func(result.Value!));
    }

    // 5. Task<Result> + Async Bind (T -> Task<Result>) -> Task<Result>
    public static async Task<Result<TNext, TError>> Bind<TValue, TNext, TError>(
        this Task<Result<TValue, TError>> resultTask,
        Func<TValue, Task<Result<TNext, TError>>> func)
        where TError : Exception
    {
        var result = await resultTask;
        if (result.IsFailure) return Result<TNext, TError>.Failure(result.Error!);
        return await func(result.Value!);
    }

    public static async Task<Result<TNext, TError>> Bind<TValue, TNext, TError>(
        this Task<Result<TValue, TError>> resultTask,
        Func<TValue, Result<TNext, TError>> func)
        where TError : Exception
    {
        var result = await resultTask;
        return result.IsFailure ? Result<TNext, TError>.Failure(result.Error!) : func(result.Value!);
    }
}
