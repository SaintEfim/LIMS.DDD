using Application.SeedWork;
using LIMS.Service.LaboratoryOperations.Domain.SampleAggregate;
using LIMS.Service.LaboratoryOperations.Domain.StudyAggregate;
using LIMS.Service.LaboratoryOperations.Domain.StudyTemplateSnapshots;
using LIMS.Service.LaboratoryOperations.Domain.UnitSnapshots;

namespace LIMS.Service.LaboratoryOperations.Application.Studies.Core;

public sealed class StudyQueries(
    IStudyRepository repository,
    IStudyTemplateSnapshotRepository snapshotRepository,
    IUnitSnapshotRepository unitSnapshotRepository) : IQueries
{
    public async Task<StudyDto?> GetByIdAsync(
        Guid id,
        CancellationToken cancellationToken = default)
    {
        var study = await repository.GetByIdAsync(new StudyId(id), cancellationToken);
        if (study is null)
        {
            return null;
        }

        var snapshot = await snapshotRepository.GetByIdAsync(study.StudyTemplateId, cancellationToken);
        if (snapshot is null)
        {
            throw new KeyNotFoundException("template not found");
        }

        var unitsById = await GetUnitsByIdAsync([snapshot], cancellationToken);

        return StudyDto.FromDomain(study, snapshot, unitsById);
    }

    public async Task<ICollection<StudyDto>> GetAllBySampleIdAsync(
        Guid sampleId,
        CancellationToken cancellationToken = default)
    {
        var studies = await repository.GetBySampleIdAsync(new SampleId(sampleId), cancellationToken);
        if (studies.Count == 0)
        {
            return [];
        }

        var snapshotsByTemplateId = new Dictionary<StudyTemplateId, StudyTemplateSnapshot>();
        foreach (var study in studies)
        {
            if (snapshotsByTemplateId.ContainsKey(study.StudyTemplateId)) continue;

            var snapshot = await snapshotRepository.GetByIdAsync(study.StudyTemplateId, cancellationToken);

            snapshotsByTemplateId[study.StudyTemplateId] = snapshot ?? throw new KeyNotFoundException("template not found");
        }

        var unitsById = await GetUnitsByIdAsync(snapshotsByTemplateId.Values, cancellationToken);

        return studies.Select(study =>
                StudyDto.FromDomain(study, snapshotsByTemplateId[study.StudyTemplateId], unitsById))
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

        if (unitIds.Count == 0)
        {
            return new Dictionary<UnitId, UnitSnapshot>();
        }

        var units = await unitSnapshotRepository.GetByIdsAsync(unitIds, cancellationToken);
        return units.ToDictionary(u => u.Id);
    }
}
