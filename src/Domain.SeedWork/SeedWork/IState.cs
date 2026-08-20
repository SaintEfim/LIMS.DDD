using Domain.SeedWork.SeedWork.Result;

namespace Domain.SeedWork.SeedWork;

public interface IState<T>
{
    string Name { get; }

    bool CanEdit { get; }

    Result<None, Exception> CanTransitionTo(
        IState<T> newState,
        T template);
}
