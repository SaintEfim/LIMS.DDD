using LIMS.DDD.Service.Domain.LaboratoryOperationsContext.OrderAggregate;
using LIMS.DDD.Service.Domain.LaboratoryOperationsContext.SampleAggregate;
using Microsoft.EntityFrameworkCore;

namespace LIMS.DDD.Service.Persistence.Repositories;

public class SampleRepository : ISampleRepository
{
    private readonly ApplicationDbContext _context;

    public SampleRepository(
        ApplicationDbContext context)
    {
        _context = context;
    }

    public async Task<Sample?> GetByIdAsync(
        SampleId id,
        CancellationToken cancellationToken = default)
    {
        return await _context.Samples.FindAsync([id], cancellationToken);
    }

    public async Task<Sample?> GetByIdForChangeAsync(
        SampleId id,
        CancellationToken cancellationToken = default)
    {
        return await _context.Samples.FindAsync([id], cancellationToken);
    }

    public async Task<ICollection<Sample>> GetAllAsync(
        CancellationToken cancellationToken = default)
    {
        var sampleQuery = await _context.Samples
            .AsNoTracking()
            .ToListAsync(cancellationToken);

        return sampleQuery;
    }

    public async Task<ICollection<Sample>> GetByOrderIdAsync(
        OrderId orderId,
        CancellationToken cancellationToken = default)
    {
        return await _context.Samples
            .AsNoTracking()
            .Where(s => s.OrderId == orderId)
            .ToListAsync(cancellationToken);
    }

    public void Add(
        Sample sample)
    {
        _context.Samples.Add(sample);
    }
}
