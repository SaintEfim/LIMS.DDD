using LIMS.Service.LaboratoryOperations.Domain.SampleAggregate;
using LIMS.Service.LaboratoryOperations.Domain.StudyAggregate;
using Microsoft.EntityFrameworkCore;

namespace LIMS.Service.LaboratoryOperations.Persistence.Repositories;

public class StudyRepository(ApplicationDbContext context) : IStudyRepository
{
    public async Task<Study?> GetByIdForChangeAsync(
        StudyId id,
        CancellationToken cancellationToken = default)
    {
        var query = context.Studies;
        return await StudyBaseQuery(query)
            .SingleOrDefaultAsync(s => s.Id == id, cancellationToken);
    }

    public async Task<Study?> GetByIdAsync(
        StudyId id,
        CancellationToken cancellationToken = default)
    {
        var query = context.Studies.AsNoTracking();
        return await StudyBaseQuery(query)
            .SingleOrDefaultAsync(s => s.Id == id, cancellationToken);
    }

    public async Task<ICollection<Study>> GetBySampleIdAsync(
        SampleId sampleId,
        CancellationToken cancellationToken = default)
    {
        return await context.Studies
            .AsNoTracking()
            .Include(s => s.MeasuredValues)
            .Include(s => s.TestResults)
            .Where(s => s.SampleId == sampleId)
            .ToListAsync(cancellationToken);
    }

    public void Add(
        Study study)
    {
        context.Studies.Add(study);
    }

    private static IQueryable<Study> StudyBaseQuery(
        IQueryable<Study> query)
    {
        return query.Include(s => s.MeasuredValues)
            .Include(s => s.TestResults);
    }
}
