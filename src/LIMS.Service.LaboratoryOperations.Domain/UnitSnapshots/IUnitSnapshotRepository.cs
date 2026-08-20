using Domain.SeedWork.SeedWork;

namespace LIMS.Service.LaboratoryOperations.Domain.UnitSnapshots;

public interface IUnitSnapshotRepository : IRepository<UnitSnapshot>
{
    Task<UnitSnapshot?> GetByIdAsync(
        UnitId id,
        CancellationToken cancellationToken = default);

    Task<ICollection<UnitSnapshot>> GetAllAsync(
        CancellationToken cancellationToken = default);

    Task<UnitSnapshot?> GetByIdForChangeAsync(
        UnitId id,
        CancellationToken cancellationToken = default);

    Task<IReadOnlyList<UnitSnapshot>> GetByIdsAsync(
        IEnumerable<UnitId> ids,
        CancellationToken cancellationToken = default);

    void Add(
        UnitSnapshot unit);
}
