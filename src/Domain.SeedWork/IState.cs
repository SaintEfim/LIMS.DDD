using Domain.SeedWork.Errors;
using Domain.SeedWork.Result;

namespace Domain.SeedWork;

public interface IState<T>
{
    string Name { get; }

    bool CanEdit { get; }

    Result<None, InvalidStatusTransitionError> CanTransitionTo(
        IState<T> newState,
        T template);
}
