using Library.Domain.SeedWork;

namespace LIMS.Service.LaboratoryOperations.Domain.BackgroundOperations;

public interface IBackgroundOperationRepository : IRepository<BackgroundOperation>
{
    Task<BackgroundOperation?> GetByIdAsync(
        Guid id,
        CancellationToken cancellationToken = default);

    Task<BackgroundOperation?> GetByIdForChangeAsync(
        Guid id,
        CancellationToken cancellationToken = default);

    Task<IReadOnlyList<BackgroundOperation>> GetPendingAsync(
        int take,
        CancellationToken cancellationToken = default);

    void Add(
        BackgroundOperation operation);
}
