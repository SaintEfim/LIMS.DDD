using System;
using System.Threading.Tasks;

namespace LIMS.DDD.Service.Domain.SeedWork.Result;

public static class ResultExtensions
{
    public static async Task<Result<TNext, TError>> Bind<T, TNext, TError>(
        this Result<T, TError> result,
        Func<T, Result<TNext, TError>> next)
    {
        return result switch
        {
            Result<T, TError>.Success s =>  next(s.Value),
            Result<T, TError>.Failure f => new Result<TNext, TError>.Failure(f.Error),
            _ => throw new InvalidOperationException("Unknown Result type")
        };
    }

    public static Result<TNext, TError> Map<T, TNext, TError>(
        this Result<T, TError> result,
        Func<T, TNext> map)
    {
        return result switch
        {
            Result<T, TError>.Success s => new Result<TNext, TError>.Success(map(s.Value)),
            Result<T, TError>.Failure f => new Result<TNext, TError>.Failure(f.Error),
            _ => throw new InvalidOperationException("Unknown Result type")
        };
    }

    public static Result<T, TError> Tee<T, TError>(
        this Result<T, TError> result,
        Action<T> action)
    {
        if (result is Result<T, TError>.Success s)
        {
            action(s.Value);
        }
        return result;
    }
}
