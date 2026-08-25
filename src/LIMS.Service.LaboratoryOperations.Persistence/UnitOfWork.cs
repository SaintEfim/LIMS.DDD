using Domain.SeedWork;

namespace LIMS.Service.LaboratoryOperations.Persistence;

internal sealed class UnitOfWork(ApplicationDbContext context) : IUnitOfWork,
    IAsyncDisposable
{
    public async Task<int> SaveChangesAsync(
        CancellationToken cancellationToken = default)
    {
        return await context.SaveChangesAsync(cancellationToken);
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

    public async ValueTask DisposeAsync()
    {
        if (context.Database.CurrentTransaction is not null)
        {
            await context.Database.CurrentTransaction.DisposeAsync();
        }
    }
}
