using Guides.Service.Domains.SeedWork;

namespace Guides.Service.Domains;

public class Unit : SoftDeletableModel
{
    public Guid Id { get; set; }

    public string Name { get; set; } = string.Empty;
}
