using Library.Domain.SeedWork;

namespace Service.Guides.Domain;

public readonly record struct UnitId(Guid Value) : IValueObjectId
{
    public Guid Value { get; } = Value;
}
