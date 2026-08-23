using Domain.SeedWork.Errors;
using Domain.SeedWork.Result;

namespace Domain.SeedWork.ValueObjects;

/// <summary>
///     Базовый класс для статусов, реализующих State Pattern
/// </summary>
public abstract record StatusBase<TState, TEntity>
    where TState : class, IState<TEntity>
{
    private readonly TState _state;

    protected StatusBase(
        TState state)
    {
        _state = state ?? throw new ArgumentNullException(nameof(state));
    }

    public string Name => _state.Name;
    public bool CanEdit => _state.CanEdit;

    public Result<None, InvalidStatusTransitionError> CanTransitionTo(
        StatusBase<TState, TEntity> newStatus,
        TEntity entity)
    {
        return _state.CanTransitionTo(newStatus._state, entity);
    }
}
