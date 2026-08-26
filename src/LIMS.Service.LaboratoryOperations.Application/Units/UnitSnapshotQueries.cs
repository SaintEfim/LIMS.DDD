using Application.SeedWork;
using LIMS.Service.LaboratoryOperations.Domain.UnitSnapshots;

namespace LIMS.Service.LaboratoryOperations.Application.Units;

public sealed class UnitSnapshotQueries(IUnitSnapshotRepository repository) : IQueries
{
    public async Task<UnitSnapshotDto?> GetByIdAsync(
        Guid id,
        CancellationToken cancellationToken = default)
    {
        var snapshot = await repository.GetByIdAsync(new UnitId(id), cancellationToken);
        return snapshot is null ? null : UnitSnapshotDto.FromSnapshot(snapshot);
    }

    public async Task<ICollection<UnitSnapshotDto>> GetAllAsync(
        CancellationToken cancellationToken = default)
    {
        var snapshots = await repository.GetAllAsync(cancellationToken);
        return snapshots.Select(UnitSnapshotDto.FromSnapshot)
            .ToList();
    }
}
