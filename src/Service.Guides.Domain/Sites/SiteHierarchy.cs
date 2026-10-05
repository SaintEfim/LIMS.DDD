using Library.Domain.SeedWork.Errors;
namespace Service.Guides.Domain.Sites;

public static class SiteHierarchy
{
    public static DomainError? ValidateParent(SiteId siteId, SiteId? parentId, IReadOnlyCollection<Site> sites)
    {
        var byId = sites.ToDictionary(site => site.Id);
        var visited = new HashSet<SiteId>();
        while (parentId is { } ancestorId)
        {
            if (ancestorId == siteId || !visited.Add(ancestorId))
                return new ValidationError("The selected parent would create a cycle in the site hierarchy.");
            if (!byId.TryGetValue(ancestorId, out var ancestor))
                return new EntityNotFoundError(nameof(Site), ancestorId.Value);
            parentId = ancestor.ParentId;
        }
        return null;
    }
}