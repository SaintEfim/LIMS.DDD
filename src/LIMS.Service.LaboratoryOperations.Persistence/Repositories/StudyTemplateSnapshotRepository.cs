using LIMS.Service.LaboratoryOperations.Domain.StudyTemplateSnapshots;
using LIMS.Service.LaboratoryOperations.Domain.StudyTemplateSnapshots.InputParameters;
using LIMS.Service.LaboratoryOperations.Domain.StudyTemplateSnapshots.ResultDefinitions;
using Microsoft.EntityFrameworkCore;

namespace LIMS.Service.LaboratoryOperations.Persistence.Repositories;

public class StudyTemplateSnapshotRepository(ApplicationDbContext context) : IStudyTemplateSnapshotRepository
{
    public async Task<ICollection<StudyTemplateSnapshot>> GetAllAsync(
        CancellationToken cancellationToken = default)
    {
        var studyTemplateQuery = context.StudyTemplates.AsNoTracking();

        var studyTemplates = await StudyTemplateSnapshotBaseQuery(studyTemplateQuery)
            .ToListAsync(cancellationToken);

        return studyTemplates;
    }

    public async Task<StudyTemplateSnapshot?> GetByIdAsync(
        StudyTemplateId id,
        CancellationToken cancellationToken = default)
    {
        var studyTemplateQuery = context.StudyTemplates.AsNoTracking();

        var studyTemplate = await StudyTemplateSnapshotBaseQuery(studyTemplateQuery)
            .SingleOrDefaultAsync(t => t.Id == id, cancellationToken);

        return studyTemplate;
    }

    public async Task<ResultDefinitionSnapshot?> GetResultDefinitionAsync(
        StudyTemplateId studyTemplateId,
        ResultDefinitionId requiredResultDefinitionId,
        CancellationToken cancellationToken = default)
    {
        return await context.StudyTemplates
            .AsNoTracking()
            .Where(t => t.Id == studyTemplateId)
            .SelectMany(t => t.Results)
            .SingleOrDefaultAsync(r => r.Id == requiredResultDefinitionId, cancellationToken);
    }

    public async Task<IReadOnlyList<ResultDefinitionSnapshot>> GetResultDefinitionsAsync(
        StudyTemplateId studyTemplateId,
        CancellationToken cancellationToken = default)
    {
        return await context.StudyTemplates
            .AsNoTracking()
            .Where(t => t.Id == studyTemplateId)
            .SelectMany(t => t.Results)
            .ToListAsync(cancellationToken);
    }

    public async Task<InputParameterSnapshot?> GetInputParameterAsync(
        StudyTemplateId studyTemplateId,
        InputParameterId requiredResultDefinitionId,
        CancellationToken cancellationToken = default)
    {
        return await context.StudyTemplates
            .AsNoTracking()
            .Where(t => t.Id == studyTemplateId)
            .SelectMany(t => t.Parameters)
            .SingleOrDefaultAsync(p => p.Id == requiredResultDefinitionId, cancellationToken);
    }

    public async Task<IReadOnlyList<InputParameterSnapshot>> GetInputParameterSnapshotsAsync(
        StudyTemplateId studyTemplateId,
        CancellationToken cancellationToken = default)
    {
        return await context.StudyTemplates
            .AsNoTracking()
            .Where(t => t.Id == studyTemplateId)
            .SelectMany(t => t.Parameters)
            .ToListAsync(cancellationToken);
    }

    public async Task<StudyTemplateSnapshot?> GetByIdForChangeAsync(
        StudyTemplateId id,
        CancellationToken cancellationToken = default)
    {
        var studyTemplateQuery = context.StudyTemplates;

        var studyTemplate = await StudyTemplateSnapshotBaseQuery(studyTemplateQuery)
            .SingleOrDefaultAsync(t => t.Id == id, cancellationToken);

        return studyTemplate;
    }

    public void Add(
        StudyTemplateSnapshot studyTemplate)
    {
        context.StudyTemplates.Add(studyTemplate);
    }

    private static IQueryable<StudyTemplateSnapshot> StudyTemplateSnapshotBaseQuery(
        IQueryable<StudyTemplateSnapshot> query)
    {
        return query.Include(t => t.Parameters)
            .Include(t => t.Results)
            .Include(x => x.CalculationRules);
    }
}
