using LIMS.DDD.Service.Domain.StudyTemplateContext.StudyTemplateAggregate;
using Microsoft.EntityFrameworkCore;

namespace LIMS.DDD.Service.Persistence.Repositories;

public class StudyTemplateRepository : IStudyTemplateRepository
{
    private readonly ApplicationDbContext _context;

    public StudyTemplateRepository(
        ApplicationDbContext context)
    {
        _context = context;
    }

    public async Task<StudyTemplate?> GetByIdAsync(
        StudyTemplateId id,
        CancellationToken cancellationToken = default)
    {
        var studyTemplateQuery = _context.StudyTemplates
            .AsSplitQuery()
            .AsNoTracking();

        var studyTemplate = await StudyTemplateBaseQuery(studyTemplateQuery)
            .SingleOrDefaultAsync(t => t.Id == id, cancellationToken);

        return studyTemplate;
    }

    public async Task<StudyTemplate?> GetByIdForChangeAsync(
        StudyTemplateId id,
        CancellationToken cancellationToken = default)
    {
        var studyTemplateQuery = _context.StudyTemplates.AsSplitQuery();

        var studyTemplate = await StudyTemplateBaseQuery(studyTemplateQuery)
            .SingleOrDefaultAsync(t => t.Id == id, cancellationToken);

        return studyTemplate;
    }

    public async Task<ICollection<StudyTemplate>> GetAllAsync(
        CancellationToken cancellationToken = default)
    {
        var studyTemplateQuery = _context.StudyTemplates
            .AsSplitQuery()
            .AsNoTracking();

        var studyTemplates = await StudyTemplateBaseQuery(studyTemplateQuery)
            .ToListAsync(cancellationToken);

        return studyTemplates;
    }

    public void Add(
        StudyTemplate studyTemplate)
    {
        _context.StudyTemplates.Add(studyTemplate);
    }

    private static IQueryable<StudyTemplate> StudyTemplateBaseQuery(
        IQueryable<StudyTemplate> query)
    {
        return query.Include(t => t.InputParameters)
            .Include(t => t.ResultDefinitions)
            .Include(t => t.CalculationRules);
    }
}
