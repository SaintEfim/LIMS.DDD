using LIMS.DDD.Service.Domain.SeedWork.Snapshots;
using LIMS.DDD.Service.Domain.StudyTemplateContext.StudyTemplateAggregate;

namespace LIMS.DDD.Service.Application.StudyTemplates.Core;

public sealed class StudyTemplateQueries(
    IStudyTemplateRepository repository,
    IUnitSnapshotRepository unitSnapshotRepository)
{
    public async Task<StudyTemplateDto?> GetByIdAsync(
        Guid id,
        CancellationToken cancellationToken = default)
    {
        var studyTemplate = await repository.GetByIdAsync(new StudyTemplateId(id), cancellationToken);
        if (studyTemplate is null) return null;

        var unitIds = studyTemplate.ResultDefinitions
            .Select(r => r.UnitId)
            .Distinct()
            .ToList();

        var units = await unitSnapshotRepository.GetByIdsAsync(unitIds, cancellationToken);
        var unitsById = units.ToDictionary(u => u.Id);

        return StudyTemplateDto.FromDomain(studyTemplate, unitsById);
    }

    public async Task<ICollection<StudyTemplateDto>> GetAllAsync(
        CancellationToken cancellationToken = default)
    {
        var studyTemplates = await repository.GetAllAsync(cancellationToken);
        if (studyTemplates.Count == 0) return [];

        var unitIds = studyTemplates.SelectMany(t => t.ResultDefinitions)
            .Select(r => r.UnitId)
            .Distinct()
            .ToList();

        var units = await unitSnapshotRepository.GetByIdsAsync(unitIds, cancellationToken);
        var unitsById = units.ToDictionary(u => u.Id);

        return studyTemplates.Select(t => StudyTemplateDto.FromDomain(t, unitsById))
            .ToList();
    }
}
