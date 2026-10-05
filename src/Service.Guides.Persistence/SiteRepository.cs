using Microsoft.EntityFrameworkCore;
using Service.Guides.Domain.Sites;

namespace Service.Guides.Persistence;

public sealed class SiteRepository(ApplicationDbContext context) : ISiteRepository
{
    public async Task LockHierarchyAsync(
        CancellationToken cancellationToken = default)
    {
        if (context.Database.CurrentTransaction is null)
            throw new InvalidOperationException("Hierarchy changes require a transaction.");
        // Every writer takes the same PostgreSQL transaction lock, across API instances.
        await context.Database.ExecuteSqlRawAsync("SELECT pg_advisory_xact_lock(734918201)", cancellationToken);
    }

    public Task<List<Site>> GetAllAsync(
        bool forChange = false,
        CancellationToken cancellationToken = default)
    {
        var query = forChange ? context.Sites : context.Sites.AsNoTracking();
        return query.OrderBy(site => site.Name)
            .ThenBy(site => site.Id)
            .ToListAsync(cancellationToken);
    }

    public Task<Site?> GetByIdAsync(
        SiteId id,
        CancellationToken cancellationToken = default) =>
        context.Sites
            .AsNoTracking()
            .SingleOrDefaultAsync(site => site.Id == id, cancellationToken);

    public void Add(
        Site site) =>
        context.Sites.Add(site);
}
