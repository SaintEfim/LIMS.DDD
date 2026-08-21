using LIMS.Service.LaboratoryOperations.Domain.OrderAggregate;
using LIMS.Service.LaboratoryOperations.Domain.SampleAggregate;
using Microsoft.EntityFrameworkCore;

namespace LIMS.Service.LaboratoryOperations.Persistence.Repositories;

public class SampleRepository(ApplicationDbContext context) : ISampleRepository
{
    public async Task<Sample?> GetByIdAsync(
        SampleId id,
        CancellationToken cancellationToken = default)
    {
        return await context.Samples.FindAsync([id], cancellationToken);
    }

    public async Task<Sample?> GetByIdForChangeAsync(
        SampleId id,
        CancellationToken cancellationToken = default)
    {
        return await context.Samples.FindAsync([id], cancellationToken);
    }

    public async Task<ICollection<Sample>> GetAllAsync(
        CancellationToken cancellationToken = default)
    {
        var sampleQuery = await context.Samples
            .AsNoTracking()
            .ToListAsync(cancellationToken);

        return sampleQuery;
    }

    public async Task<ICollection<Sample>> GetByOrderIdAsync(
        OrderId orderId,
        CancellationToken cancellationToken = default)
    {
        return await context.Samples
            .AsNoTracking()
            .Where(s => s.OrderId == orderId)
            .ToListAsync(cancellationToken);
    }

    public void Add(
        Sample sample)
    {
        context.Samples.Add(sample);
    }
}
