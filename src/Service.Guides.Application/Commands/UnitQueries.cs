using Library.Application.SeedWork;
using Service.Guides.Domain;

namespace Service.Guides.Application.Commands;

public sealed class UnitQueries(IUnitRepository repository) : IQueries
{
    public async Task<UnitDto?> GetByIdAsync(
        Guid id,
        CancellationToken cancellationToken = default)
    {
        var snapshot = await repository.GetByIdAsync(new UnitId(id), cancellationToken);
        return snapshot is null ? null : UnitDto.FromDomain(snapshot);
    }

    public async Task<ICollection<UnitDto>> GetAllAsync(
        CancellationToken cancellationToken = default)
    {
        var snapshots = await repository.GetAllAsync(cancellationToken);
        return snapshots.Select(UnitDto.FromDomain)
            .ToList();
    }
}
