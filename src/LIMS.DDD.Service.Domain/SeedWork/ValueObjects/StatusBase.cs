using LIMS.DDD.Service.Domain.SeedWork.Result;

namespace LIMS.DDD.Service.Domain.SeedWork.ValueObjects;

/// <summary>
/// Базовый класс для статусов, реализующих State Pattern
/// </summary>
public abstract record StatusBase<TState, TEntity>
    where TState : class, IState<TEntity>
{
    protected readonly TState State;

    protected StatusBase(
        TState state)
    {
        State = state ?? throw new ArgumentNullException(nameof(state));
    }

    private string Name => State.Name;
    public bool CanEdit => State.CanEdit;

    public Result<Exception> CanTransitionTo(
        StatusBase<TState, TEntity> newStatus,
        TEntity entity)
    {
        return State.CanTransitionTo(newStatus.State, entity);
    }

    public override string ToString() => Name;
}
