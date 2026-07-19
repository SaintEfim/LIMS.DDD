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
        return await _context.StudyTemplates
            .AsNoTracking()
            .Include(t => t.Parameters)
            .Include(t => t.Results)
            .FirstOrDefaultAsync(t => t.Id == id, cancellationToken);
    }

    public async Task<ICollection<StudyTemplate>> GetAllAsync(
        CancellationToken cancellationToken = default)
    {
        return await _context.StudyTemplates
            .AsNoTracking()
            .Include(t => t.Parameters)
            .Include(t => t.Results)
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
        StudyTemplate entity,
        CancellationToken cancellationToken = default)
    {
        _context.StudyTemplates.Update(entity);
    }

    public async Task<int> SaveChangesAsync(
        CancellationToken cancellationToken = default)
    {
        return await _context.SaveChangesAsync(cancellationToken);
    }
}
