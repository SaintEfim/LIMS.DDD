using Library.Domain.SeedWork.Errors;
using Library.Domain.SeedWork.Result;

namespace Library.Domain.SeedWork;

public interface IState<T>
{
    string Name { get; }

    bool CanEdit { get; }

    Result<None, InvalidStatusTransitionError> CanTransitionTo(
        IState<T> newState,
        T template);
}
