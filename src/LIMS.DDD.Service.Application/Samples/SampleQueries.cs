using LIMS.DDD.Service.Application.LaboratoryOperations.Samples.Queries;
using LIMS.DDD.Service.Domain.LaboratoryOperationsContext.SampleAggregate;

namespace LIMS.DDD.Service.Application.Samples;

public sealed class SampleQueries(ISampleRepository repository)
{
    public async Task<SampleDto?> GetByIdAsync(
        Guid id,
        CancellationToken cancellationToken = default)
    {
        var sample = await repository.GetByIdAsync(new SampleId(id), cancellationToken);
        return sample is null ? null : SampleDto.FromDomain(sample);
    }
}
