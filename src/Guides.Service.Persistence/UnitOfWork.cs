using Library.Domain.SeedWork;
using Library.Domain.SeedWork.Events;

namespace Guides.Service.Persistence;

internal sealed class UnitOfWork(ApplicationDbContext context, IDomainEventsDispatcher domainEventsDispatcher)
    : IUnitOfWork,
        IAsyncDisposable
{
    public async ValueTask DisposeAsync()
    {
        if (context.Database.CurrentTransaction is not null)
        {
            await context.Database.CurrentTransaction.DisposeAsync();
        }
    }

    public async Task<int> SaveChangesAsync(
        CancellationToken cancellationToken = default)
    {
        var domainEvents = CollectDomainEvents();

        await domainEventsDispatcher.DispatchAsync(domainEvents, cancellationToken);

        ClearDomainEvents();

        return await context.SaveChangesAsync(cancellationToken);
    }

    private IReadOnlyList<IDomainEvent> CollectDomainEvents()
    {
        return context.ChangeTracker
            .Entries<EntityBase>()
            .Where(entry => entry.Entity.DomainEvents.Count > 0)
            .SelectMany(entry => entry.Entity.DomainEvents)
            .ToList();
    }

    private void ClearDomainEvents()
    {
        var entityChangeTracker = context.ChangeTracker
            .Entries<EntityBase>()
            .Where(entry => entry.Entity.DomainEvents.Count > 0);

        foreach (var entry in entityChangeTracker)
        {
            entry.Entity.ClearDomainEvents();
        }
    }

    public async Task BeginTransactionAsync(
        CancellationToken cancellationToken = default)
    {
        if (context.Database.CurrentTransaction is not null)
        {
            return;
        }

        await context.Database.BeginTransactionAsync(cancellationToken);
    }

    public async Task CommitTransactionAsync(
        CancellationToken cancellationToken = default)
    {
        if (context.Database.CurrentTransaction is null)
        {
            return;
        }

        await context.Database.CommitTransactionAsync(cancellationToken);
    }

    public async Task RollbackTransactionAsync(
        CancellationToken cancellationToken = default)
    {
        if (context.Database.CurrentTransaction is null)
        {
            return;
        }

        await context.Database.RollbackTransactionAsync(cancellationToken);
    }
}
