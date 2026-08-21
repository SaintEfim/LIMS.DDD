using Application.SeedWork.SeedWork;
using LIMS.Service.LaboratoryOperations.Domain.UnitSnapshots;

namespace LIMS.Service.LaboratoryOperations.Application.Units;

public sealed class UnitSnapshotQueries(IUnitSnapshotRepository repository) : IQueries
{
    public async Task<UnitDto?> GetByIdAsync(
        Guid id,
        CancellationToken cancellationToken = default)
    {
        var snapshot = await repository.GetByIdAsync(new UnitId(id), cancellationToken);
        return snapshot is null ? null : UnitDto.FromSnapshot(snapshot);
    }

    public async Task<ICollection<UnitDto>> GetAllAsync(
        CancellationToken cancellationToken = default)
    {
        var snapshots = await repository.GetAllAsync(cancellationToken);
        return snapshots.Select(UnitDto.FromSnapshot).ToList();
    }
}
