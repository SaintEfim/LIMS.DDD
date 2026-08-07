using LIMS.DDD.Service.Domain.LaboratoryOperationsContext.OrderAggregate;
using LIMS.DDD.Service.Domain.SeedWork;

namespace LIMS.DDD.Service.Domain.LaboratoryOperationsContext.SampleAggregate;

public interface ISampleRepository : IRepository<Sample>
{
    Task<Sample?> GetByIdAsync(
        SampleId id,
        CancellationToken cancellationToken = default);

    Task<Sample?> GetByIdForChangeAsync(
        SampleId id,
        CancellationToken cancellationToken = default);

    Task<ICollection<Sample>> GetAllAsync(
        CancellationToken cancellationToken = default);

    Task<ICollection<Sample>> GetByOrderIdAsync(
        OrderId orderId,
        CancellationToken cancellationToken = default);

    void Add(
        Sample sample);

    Task<int> SaveChangesAsync(
        CancellationToken cancellationToken = default);
}
