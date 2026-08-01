using LIMS.DDD.Service.Domain.SeedWork.Result;

namespace LIMS.DDD.Service.Domain.SeedWork;

public interface IState<T>
{
    string Name { get; }

    bool CanEdit { get; }

    Result<Exception> CanTransitionTo(
        IState<T> newState,
        T template);
}
