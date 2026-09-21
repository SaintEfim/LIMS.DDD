using LIMS.Service.LaboratoryOperations.Domain.BackgroundOperations;
using Microsoft.EntityFrameworkCore;

namespace LIMS.Service.LaboratoryOperations.Persistence.Repositories;

public sealed class BackgroundOperationRepository(ApplicationDbContext context) : IBackgroundOperationRepository
{
    public Task<BackgroundOperation?> GetByIdAsync(
        Guid id,
        CancellationToken cancellationToken = default) =>
        context.BackgroundOperations
            .AsNoTracking()
            .SingleOrDefaultAsync(operation => operation.Id == id, cancellationToken);

    public Task<BackgroundOperation?> GetByIdForChangeAsync(
        Guid id,
        CancellationToken cancellationToken = default) =>
        context.BackgroundOperations.SingleOrDefaultAsync(operation => operation.Id == id, cancellationToken);

    public async Task<IReadOnlyList<BackgroundOperation>> GetPendingAsync(
        int take,
        CancellationToken cancellationToken = default) =>
        await context.BackgroundOperations
            .AsNoTracking()
            .Where(operation => operation.Status == OperationStatus.Pending)
            .OrderBy(operation => operation.CreatedAtUtc)
            .Take(take)
            .ToListAsync(cancellationToken);

    public void Add(
        BackgroundOperation operation) =>
        context.BackgroundOperations.Add(operation);
}
