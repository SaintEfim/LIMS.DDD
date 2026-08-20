using LIMS.Service.Methodologies.Domain.UnitSnapshots;
using Microsoft.EntityFrameworkCore;

namespace LIMS.Service.Methodologies.Persistence.Repositories;

public sealed class UnitSnapshotRepository(ApplicationDbContext context) : IUnitSnapshotRepository
{
    public async Task<IReadOnlyList<UnitSnapshot>> GetByIdsAsync(
        IEnumerable<UnitId> ids,
        CancellationToken cancellationToken = default)
    {
        var idList = ids.ToList();
        if (idList.Count == 0)
        {
            return [];
        }

        return await context.UnitSnapshots
            .AsNoTracking()
            .Where(u => idList.Contains(u.Id))
            .ToListAsync(cancellationToken);
    }

    public void Add(
        UnitSnapshot unit)
    {
        context.UnitSnapshots.Add(unit);
    }

    public async Task<ICollection<UnitSnapshot>> GetAllAsync(
        CancellationToken cancellationToken = default)
    {
        return await context.UnitSnapshots
            .AsNoTracking()
            .ToListAsync(cancellationToken);
    }

    public async Task<UnitSnapshot?> GetByIdAsync(
        UnitId id,
        CancellationToken cancellationToken = default)
    {
        return await context.UnitSnapshots
            .AsNoTracking()
            .SingleOrDefaultAsync(x => x.Id == id, cancellationToken);
    }

    public async Task<UnitSnapshot?> GetByIdForChangeAsync(
        UnitId id,
        CancellationToken cancellationToken = default)
    {
        return await context.UnitSnapshots.SingleOrDefaultAsync(x => x.Id == id, cancellationToken);
    }
}
