using Domain.SeedWork;
using LIMS.Service.LaboratoryOperations.Domain.OrderAggregate;

namespace LIMS.Service.LaboratoryOperations.Domain.SampleAggregate;

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
}
