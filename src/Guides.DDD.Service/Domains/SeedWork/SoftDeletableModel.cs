namespace Guides.DDD.Service.Domains.SeedWork;

public abstract class SoftDeletableModel
{
    public bool IsDeleted { get; protected internal set; }

    public DateTimeOffset? DeletedAt { get; protected set; }
}
