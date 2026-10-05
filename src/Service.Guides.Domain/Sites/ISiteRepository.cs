using Library.Domain.SeedWork;
namespace Service.Guides.Domain.Sites;

public interface ISiteRepository : IRepository<Site>
{
    // Called inside a transaction before reading or changing the hierarchy.
    Task LockHierarchyAsync(CancellationToken cancellationToken = default);
    Task<List<Site>> GetAllAsync(bool forChange = false, CancellationToken cancellationToken = default);
    Task<Site?> GetByIdAsync(SiteId id, CancellationToken cancellationToken = default);
    void Add(Site site);
}