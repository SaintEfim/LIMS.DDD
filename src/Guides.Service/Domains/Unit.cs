using Library.Domain.SeedWork;

namespace Guides.Service.Domains;

public class Unit : SoftDeletableModel
{
    public Guid Id { get; set; }

    public string Name { get; set; } = string.Empty;

    public void MarkAsDeleted()
    {
        IsDeleted = true;
        DeletedAt = DateTimeOffset.UtcNow;
    }
}
