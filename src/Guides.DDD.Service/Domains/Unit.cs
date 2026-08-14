using Guides.DDD.Service.Domains.SeedWork;

namespace Guides.DDD.Service.Domains;

public class Unit : SoftDeletableModel
{
    public Guid Id { get; set; }

    public string Name { get; set; } = string.Empty;
}
