using Application.SeedWork.SeedWork;
using LIMS.Service.LaboratoryOperations.Domain.StudyTemplateSnapshots;
using LIMS.Service.LaboratoryOperations.Domain.UnitSnapshots;

namespace LIMS.Service.LaboratoryOperations.Application.StudyTemplates;

public sealed class StudyTemplateQueries(
    IStudyTemplateSnapshotRepository repository,
    IUnitSnapshotRepository unitSnapshotRepository) : IQueries
{
    public async Task<StudyTemplateDto?> GetByIdAsync(
        Guid id,
        CancellationToken cancellationToken = default)
    {
        var snapshot = await repository.GetByIdAsync(new StudyTemplateId(id), cancellationToken);
        if (snapshot is null) return null;

        var unitsById = await GetUnitsByIdAsync([snapshot], cancellationToken);
        return StudyTemplateDto.FromSnapshot(snapshot, unitsById);
    }

    public async Task<ICollection<StudyTemplateDto>> GetAllAsync(
        CancellationToken cancellationToken = default)
    {
        var snapshots = await repository.GetAllAsync(cancellationToken);
        if (snapshots.Count == 0) return [];

        var unitsById = await GetUnitsByIdAsync(snapshots, cancellationToken);
        return snapshots.Select(s => StudyTemplateDto.FromSnapshot(s, unitsById))
            .ToList();
    }

    private async Task<IReadOnlyDictionary<UnitId, UnitSnapshot>> GetUnitsByIdAsync(
        IEnumerable<StudyTemplateSnapshot> snapshots,
        CancellationToken cancellationToken)
    {
        var unitIds = snapshots.SelectMany(s => s.Results)
            .Select(r => r.UnitId)
            .Distinct()
            .ToList();

        if (unitIds.Count == 0) return new Dictionary<UnitId, UnitSnapshot>();

        var units = await unitSnapshotRepository.GetByIdsAsync(unitIds, cancellationToken);
        return units.ToDictionary(u => u.Id);
    }
}
