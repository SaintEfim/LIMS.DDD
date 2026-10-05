using Library.Domain.SeedWork;
using Library.Domain.SeedWork.ValueObjects;
namespace Service.Guides.Domain.Sites;

public sealed class Site : EntityBase, IAggregateRoot, ISoftDeletable
{
    private Site() { }
    public Site(Name name, SiteId? parentId = null)
    {
        Id = new SiteId(Guid.NewGuid());
        Name = name;
        ParentId = parentId;
    }
    public SiteId Id { get; private set; }
    public Name Name { get; private set; } = null!;
    public SiteId? ParentId { get; private set; }
    public bool IsDeleted { get; private set; }
    public DateTimeOffset? DeletedAt { get; private set; }
    public void Rename(Name name) => Name = name;
    // The complete parent chain must be validated by SiteHierarchy before this change.
    public void SetParent(SiteId? parentId)
    {
        if (parentId == Id) throw new ArgumentException("A site cannot be its own parent.", nameof(parentId));
        ParentId = parentId;
    }
    public void MarkAsDeleted()
    {
        IsDeleted = true;
        DeletedAt = DateTimeOffset.UtcNow;
    }
}