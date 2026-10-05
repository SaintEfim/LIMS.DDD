using Library.Domain.SeedWork;
namespace Service.Guides.Domain.Sites;
public readonly record struct SiteId(Guid Value) : IValueObjectId;