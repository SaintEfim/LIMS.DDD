using LIMS.DDD.Service.Domain;
using LIMS.DDD.Service.Domain.StudyTemplateAggregate;
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
        var studyTemplate = await _context.StudyTemplates
            .AsSplitQuery()
            .AsNoTracking()
            .Include(t => t.InputParameters)
            .SingleOrDefaultAsync(t => t.Id == id, cancellationToken);

        return studyTemplate;
    }

    public async Task<StudyTemplate?> GetByIdForChangeAsync(
        StudyTemplateId id,
        CancellationToken cancellationToken = default)
    {
        var studyTemplate = await _context.StudyTemplates
            .AsSplitQuery()
            .Include(t => t.InputParameters)
            .SingleOrDefaultAsync(t => t.Id == id, cancellationToken);

        return studyTemplate;
    }

    public async Task<ICollection<StudyTemplate>> GetAllAsync(
        CancellationToken cancellationToken = default)
    {
        return await _context.StudyTemplates
            .AsSplitQuery()
            .AsNoTracking()
            .Include(t => t.InputParameters)
            .ToListAsync(cancellationToken);
    }

    public void Add(
        StudyTemplate studyTemplate)
    {
        _context.StudyTemplates.Add(studyTemplate);
    }

    public void Remove(
        StudyTemplate studyTemplate)
    {
        _context.StudyTemplates.Remove(studyTemplate);
    }

    public void Update(
        StudyTemplate entity)
    {
        _context.StudyTemplates.Update(entity);
    }

    public async Task<int> SaveChangesAsync(
        CancellationToken cancellationToken = default)
    {
        return await _context.SaveChangesAsync(cancellationToken);
    }

    public async Task<bool> ExistsByNameAndRevisionAsync(
        Name name,
        Revision revision,
        CancellationToken cancellationToken = default)
    {
        return await _context.StudyTemplates.AnyAsync(x => x.Name == name && x.Revision == revision,
            cancellationToken: cancellationToken);
    }
}
