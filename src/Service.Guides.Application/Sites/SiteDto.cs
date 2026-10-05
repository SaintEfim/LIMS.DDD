using Service.Guides.Domain.Sites;
namespace Service.Guides.Application.Sites;

public sealed record SiteDto(Guid Id, string Name, Guid? ParentId)
{
    public static SiteDto FromDomain(Site site) => new(site.Id.Value, site.Name.Value, site.ParentId?.Value);
}
public sealed record SiteTreeDto(Guid Id, string Name, Guid? ParentId, List<SiteTreeDto> Children);
public sealed record CreateSiteCommand(string Name, Guid? ParentId = null);
public sealed record UpdateSiteCommand(string Name, Guid? ParentId = null);