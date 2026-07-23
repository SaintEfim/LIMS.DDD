using System;

namespace LIMS.DDD.Service.Domain.SeedWork.Result;

public abstract record Result<TSuccess, TError>
{
    private Result() { }

    public sealed record Success(TSuccess Value) : Result<TSuccess, TError>;
    public sealed record Failure(TError Error) : Result<TSuccess, TError>;
}
