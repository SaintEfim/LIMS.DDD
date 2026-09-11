using Microsoft.EntityFrameworkCore;
using Guides.Service.Domain;

namespace Guides.Service.Persistence;

public sealed class UnitRepository(ApplicationDbContext context) : IUnitRepository
{
    public async Task<Unit?> GetByIdForChangeAsync(
        UnitId id,
        CancellationToken cancellationToken = default)
    {
        return await context.Units.SingleOrDefaultAsync(x => x.Id == id, cancellationToken);
    }

    public async Task<IReadOnlyList<Unit>> GetByIdsAsync(
        IEnumerable<UnitId> ids,
        CancellationToken cancellationToken = default)
    {
        var idList = ids.ToList();
        if (idList.Count == 0)
        {
            return [];
        }

        return await context.Units
            .AsNoTracking()
            .Where(u => idList.Contains(u.Id))
            .ToListAsync(cancellationToken);
    }

    public void Add(
        Unit unit)
    {
        context.Units.Add(unit);
    }

    public async Task<ICollection<Unit>> GetAllAsync(
        CancellationToken cancellationToken = default)
    {
        return await context.Units
            .AsNoTracking()
            .ToListAsync(cancellationToken);
    }

    public async Task<Unit?> GetByIdAsync(
        UnitId id,
        CancellationToken cancellationToken = default)
    {
        return await context.Units
            .AsNoTracking()
            .SingleOrDefaultAsync(x => x.Id == id, cancellationToken);
    }
}
