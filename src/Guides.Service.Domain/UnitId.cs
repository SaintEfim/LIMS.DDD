using Library.Domain.SeedWork;

namespace Guides.Service.Domain;

public readonly record struct UnitId(Guid Value) : IValueObjectId
{
    public Guid Value { get; } = Value;
}
