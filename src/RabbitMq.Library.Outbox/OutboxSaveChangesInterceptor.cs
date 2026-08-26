using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Diagnostics;

namespace RabbitMq.Library.Outbox;

public sealed class OutboxSaveChangesInterceptor(OutboxSignal signal) : SaveChangesInterceptor
{
    private bool _hasNewOutboxMessages;

    public override InterceptionResult<int> SavingChanges(
        DbContextEventData eventData,
        InterceptionResult<int> result)
    {
        _hasNewOutboxMessages = HasNewOutboxMessages(eventData.Context);

        return result;
    }

    public override async ValueTask<InterceptionResult<int>> SavingChangesAsync(
        DbContextEventData eventData,
        InterceptionResult<int> result,
        CancellationToken cancellationToken = default)
    {
        _hasNewOutboxMessages = HasNewOutboxMessages(eventData.Context);

        return await new ValueTask<InterceptionResult<int>>(result);
    }

    public override int SavedChanges(
        SaveChangesCompletedEventData eventData,
        int result)
    {
        NotifyIfNeeded();

        return result;
    }

    public override async ValueTask<int> SavedChangesAsync(
        SaveChangesCompletedEventData eventData,
        int result,
        CancellationToken cancellationToken = default)
    {
        NotifyIfNeeded();

        return await new ValueTask<int>(result);
    }

    private static bool HasNewOutboxMessages(
        DbContext? context)
    {
        return context?.ChangeTracker
            .Entries<OutboxMessage>()
            .Any(x => x.State == EntityState.Added) ?? false;
    }

    private void NotifyIfNeeded()
    {
        if (!_hasNewOutboxMessages)
        {
            return;
        }

        signal.Notify();
        _hasNewOutboxMessages = false;
    }
}
