using LIMS.Service.Methodologies.Domain.SeedWork.Result;

namespace LIMS.Service.Methodologies.Domain.SeedWork;

public interface IState<T>
{
    string Name { get; }

    bool CanEdit { get; }

    Result<None, Exception> CanTransitionTo(
        IState<T> newState,
        T template);
}
