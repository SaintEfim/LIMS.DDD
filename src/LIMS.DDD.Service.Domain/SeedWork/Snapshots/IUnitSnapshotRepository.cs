namespace LIMS.DDD.Service.Domain.SeedWork.Snapshots;

public interface IUnitSnapshotRepository
{
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
