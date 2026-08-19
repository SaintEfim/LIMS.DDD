using LIMS.Service.Methodologies.Domain.StudyTemplateAggregate;
using LIMS.Service.Methodologies.Domain.StudyTemplateAggregate.Entities.InputParameters;
using LIMS.Service.Methodologies.Domain.StudyTemplateAggregate.Entities.ResultDefinitions;
using Microsoft.EntityFrameworkCore;

namespace LIMS.Service.Methodologies.Persistence.Repositories;

public class StudyTemplateRepository(ApplicationDbContext context) : IStudyTemplateRepository
{
    public async Task<StudyTemplate?> GetByIdAsync(
        StudyTemplateId id,
        CancellationToken cancellationToken = default)
    {
        var studyTemplateQuery = context.StudyTemplates.AsNoTracking();

        var studyTemplate = await StudyTemplateBaseQuery(studyTemplateQuery)
            .SingleOrDefaultAsync(t => t.Id == id, cancellationToken);

        return studyTemplate;
    }

    public async Task<ResultDefinition?> GetResultDefinitionAsync(
        StudyTemplateId studyTemplateId,
        ResultDefinitionId resultDefinitionId,
        CancellationToken cancellationToken = default)
    {
        return await context.StudyTemplates
            .AsNoTracking()
            .Where(t => t.Id == studyTemplateId)
            .SelectMany(t => t.ResultDefinitions)
            .SingleOrDefaultAsync(r => r.Id == resultDefinitionId, cancellationToken);
    }

    public async Task<IReadOnlyList<ResultDefinition>> GetResultDefinitionsAsync(
        StudyTemplateId studyTemplateId,
        CancellationToken cancellationToken = default)
    {
        return await context.StudyTemplates
            .AsNoTracking()
            .Where(t => t.Id == studyTemplateId)
            .SelectMany(t => t.ResultDefinitions)
            .ToListAsync(cancellationToken);
    }

    public async Task<InputParameter?> GetInputParameterAsync(
        StudyTemplateId studyTemplateId,
        InputParameterId requiredResultDefinitionId,
        CancellationToken cancellationToken = default)
    {
        return await context.StudyTemplates
            .AsNoTracking()
            .Where(t => t.Id == studyTemplateId)
            .SelectMany(t => t.InputParameters)
            .SingleOrDefaultAsync(p => p.Id == requiredResultDefinitionId, cancellationToken);
    }

    public async Task<IReadOnlyList<InputParameter>> GetInputParameterSnapshotsAsync(
        StudyTemplateId studyTemplateId,
        CancellationToken cancellationToken = default)
    {
        return await context.StudyTemplates
            .AsNoTracking()
            .Where(t => t.Id == studyTemplateId)
            .SelectMany(t => t.InputParameters)
            .ToListAsync(cancellationToken);
    }

    public async Task<StudyTemplate?> GetByIdForChangeAsync(
        StudyTemplateId id,
        CancellationToken cancellationToken = default)
    {
        var studyTemplateQuery = context.StudyTemplates;

        var studyTemplate = await StudyTemplateBaseQuery(studyTemplateQuery)
            .SingleOrDefaultAsync(t => t.Id == id, cancellationToken);

        return studyTemplate;
    }

    public async Task<ICollection<StudyTemplate>> GetAllAsync(
        CancellationToken cancellationToken = default)
    {
        var studyTemplateQuery = context.StudyTemplates.AsNoTracking();

        var studyTemplates = await StudyTemplateBaseQuery(studyTemplateQuery)
            .ToListAsync(cancellationToken);

        return studyTemplates;
    }

    public void Add(
        StudyTemplate studyTemplate)
    {
        context.StudyTemplates.Add(studyTemplate);
    }

    private static IQueryable<StudyTemplate> StudyTemplateBaseQuery(
        IQueryable<StudyTemplate> query)
    {
        return query.Include(t => t.InputParameters)
            .Include(t => t.ResultDefinitions)
            .Include(t => t.CalculationRules);
    }
}
