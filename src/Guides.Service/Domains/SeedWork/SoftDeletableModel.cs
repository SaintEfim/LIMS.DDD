namespace Guides.Service.Domains.SeedWork;

public abstract class SoftDeletableModel
{
    public bool IsDeleted { get; set; }

    public DateTimeOffset? DeletedAt { get; set; }
}
