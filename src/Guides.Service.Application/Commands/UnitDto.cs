using Guides.Service.Domain;

namespace Guides.Service.Application.Commands;

public sealed record UnitDto(Guid Id, string Name)
{
    public static UnitDto FromDomain(
        Unit unit)
    {
        return new UnitDto(unit.Id.Value, unit.Name.Value);
    }
}
