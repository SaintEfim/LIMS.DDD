using Library.Application.SeedWork;
using Service.Guides.Domain.Sites;
namespace Service.Guides.Application.Sites;

public sealed class SiteQueries(ISiteRepository repository) : IQueries
{
    public async Task<SiteDto?> GetByIdAsync(Guid id, CancellationToken cancellationToken = default)
    {
        var site = await repository.GetByIdAsync(new SiteId(id), cancellationToken);
        return site is null ? null : SiteDto.FromDomain(site);
    }
    public async Task<List<SiteDto>> GetAllAsync(CancellationToken cancellationToken = default) =>
        (await repository.GetAllAsync(cancellationToken: cancellationToken)).Select(SiteDto.FromDomain).ToList();
    public async Task<List<SiteTreeDto>> GetTreeAsync(CancellationToken cancellationToken = default)
    {
        var sites = await GetAllAsync(cancellationToken);
        var nodes = sites.ToDictionary(site => site.Id, site => new SiteTreeDto(site.Id, site.Name, site.ParentId, []));
        var roots = new List<SiteTreeDto>();
        foreach (var site in sites)
        {
            var node = nodes[site.Id];
            if (site.ParentId is { } parentId) nodes[parentId].Children.Add(node);
            else roots.Add(node);
        }
        return roots;
    }
}