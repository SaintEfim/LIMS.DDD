using Domain.SeedWork.Result;

namespace Domain.SeedWork;

public interface IState<T>
{
    string Name { get; }

    bool CanEdit { get; }

    Result<None, Exception> CanTransitionTo(
        IState<T> newState,
        T template);
}
