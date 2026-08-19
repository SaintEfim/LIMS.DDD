using LIMS.Service.Methodologies.Domain.SeedWork;

namespace LIMS.Service.Methodologies.Domain.UnitSnapshots;

public interface IUnitSnapshotRepository : IRepository<UnitSnapshot>
{
    Task<ICollection<UnitSnapshot>> GetAllAsync(
        CancellationToken cancellationToken = default);

    Task<UnitSnapshot?> GetByIdAsync(
        UnitId id,
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
