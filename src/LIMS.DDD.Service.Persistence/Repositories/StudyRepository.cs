using LIMS.DDD.Service.Domain.LaboratoryOperationsContext.SampleAggregate;
using LIMS.DDD.Service.Domain.LaboratoryOperationsContext.StudyAggregate;
using Microsoft.EntityFrameworkCore;

namespace LIMS.DDD.Service.Persistence.Repositories;

public class StudyRepository : IStudyRepository
{
    private readonly ApplicationDbContext _context;

    public StudyRepository(
        ApplicationDbContext context)
    {
        _context = context;
    }

    public async Task<Study?> GetByIdForChangeAsync(
        StudyId id,
        CancellationToken cancellationToken = default)
    {
        var query = _context.Studies.AsSplitQuery();
        return await StudyBaseQuery(query)
            .SingleOrDefaultAsync(s => s.Id == id, cancellationToken);
    }

    public async Task<Study?> GetByIdAsync(
        StudyId id,
        CancellationToken cancellationToken = default)
    {
        var query = _context.Studies
            .AsSplitQuery()
            .AsNoTracking();
        return await StudyBaseQuery(query)
            .SingleOrDefaultAsync(s => s.Id == id, cancellationToken);
    }

    public async Task<ICollection<Study>> GetBySampleIdAsync(
        SampleId sampleId,
        CancellationToken cancellationToken = default)
    {
        return await _context.Studies
            .AsNoTracking()
            .AsSplitQuery()
            .Include(s => s.MeasuredValues)
            .Include(s => s.TestResults)
            .Where(s => s.SampleId == sampleId)
            .ToListAsync(cancellationToken);
    }

    public void Add(
        Study study)
    {
        _context.Studies.Add(study);
    }

    public async Task<int> SaveChangesAsync(
        CancellationToken cancellationToken = default)
    {
        return await _context.SaveChangesAsync(cancellationToken);
    }

    private static IQueryable<Study> StudyBaseQuery(
        IQueryable<Study> query)
    {
        return query.Include(s => s.MeasuredValues)
            .Include(s => s.TestResults);
    }
}
