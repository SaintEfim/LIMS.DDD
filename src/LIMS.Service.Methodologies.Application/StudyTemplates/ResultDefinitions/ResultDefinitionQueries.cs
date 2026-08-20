using LIMS.Service.Methodologies.Domain.StudyTemplateAggregate;
using LIMS.Service.Methodologies.Domain.StudyTemplateAggregate.Entities.ResultDefinitions;
using LIMS.Service.Methodologies.Domain.UnitSnapshots;

namespace LIMS.Service.Methodologies.Application.StudyTemplates.ResultDefinitions;

public sealed class ResultDefinitionQueries(
    IStudyTemplateRepository repository,
    IUnitSnapshotRepository unitSnapshotRepository)
{
    public async Task<ResultDefinitionDto?> GetByIdAsync(
        Guid studyTemplateId,
        Guid resultId,
        CancellationToken cancellationToken = default)
    {
        var result = await repository.GetResultDefinitionAsync(new StudyTemplateId(studyTemplateId),
            new ResultDefinitionId(resultId), cancellationToken);

        if (result is null)
        {
            return null;
        }

        var unit = await unitSnapshotRepository.GetByIdAsync(result.UnitId, cancellationToken);
        return unit is null
            ? throw new KeyNotFoundException($"Unit with id {result.UnitId} not found.")
            : ResultDefinitionDto.FromDomain(unit, result);
    }

    public async Task<ICollection<ResultDefinitionDto>> GetAllByTemplateIdAsync(
        Guid studyTemplateId,
        CancellationToken cancellationToken = default)
    {
        var results = await repository.GetResultDefinitionsAsync(
            new StudyTemplateId(studyTemplateId), cancellationToken);

        if (results.Count == 0)
        {
            return [];
        }

        var unitIds = results.Select(r => r.UnitId)
            .Distinct()
            .ToList();
        var units = await unitSnapshotRepository.GetByIdsAsync(unitIds, cancellationToken);

        var unitsById = units.ToDictionary(u => u.Id);

        return results.Select(result =>
            {
                unitsById.TryGetValue(result.UnitId, out var unit);

                return unit is null
                    ? throw new KeyNotFoundException($"Unit with id {result.UnitId} not found.")
                    : ResultDefinitionDto.FromDomain(unit, result);
            })
            .ToList();
    }
}
