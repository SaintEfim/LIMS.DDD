using LIMS.Service.LaboratoryOperations.Domain.OrderAggregate;
using LIMS.Service.LaboratoryOperations.Domain.SampleAggregate;

namespace LIMS.Service.LaboratoryOperations.Application.Samples;

public sealed class SampleQueries(ISampleRepository repository)
{
    public async Task<SampleDto?> GetByIdAsync(
        Guid id,
        CancellationToken cancellationToken = default)
    {
        var sample = await repository.GetByIdAsync(new SampleId(id), cancellationToken);
        return sample is null ? null : SampleDto.FromDomain(sample);
    }

    public async Task<ICollection<SampleDto>> GetAllByOrderIdAsync(
        Guid orderId,
        CancellationToken cancellationToken = default)
    {
        var samples = await repository.GetByOrderIdAsync(new OrderId(orderId), cancellationToken);

        return samples.Select(SampleDto.FromDomain)
            .ToList();
    }
}
