using Library.Domain.SeedWork;

namespace Guides.Service.Domain;

public interface IUnitRepository : IRepository<Unit>
{
    Task<ICollection<Unit>> GetAllAsync(
        CancellationToken cancellationToken = default);

    Task<Unit?> GetByIdAsync(
        UnitId id,
        CancellationToken cancellationToken = default);

    Task<Unit?> GetByIdForChangeAsync(
        UnitId id,
        CancellationToken cancellationToken = default);

    Task<IReadOnlyList<Unit>> GetByIdsAsync(
        IEnumerable<UnitId> ids,
        CancellationToken cancellationToken = default);

    void Add(
        Unit unit);
}
