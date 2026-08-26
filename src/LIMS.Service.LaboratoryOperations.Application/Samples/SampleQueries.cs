using Application.SeedWork;
using LIMS.Service.LaboratoryOperations.Domain.OrderAggregate;
using LIMS.Service.LaboratoryOperations.Domain.SampleAggregate;
using LIMS.Service.LaboratoryOperations.Domain.UnitSnapshots;

namespace LIMS.Service.LaboratoryOperations.Application.Samples;

public sealed class SampleQueries(
    ISampleRepository sampleRepository,
    IUnitSnapshotRepository unitSnapshotRepository) : IQueries
{
    public async Task<SampleDto?> GetByIdAsync(
        Guid id,
        CancellationToken cancellationToken = default)
    {
        var sample = await sampleRepository.GetByIdAsync(new SampleId(id), cancellationToken);
        if (sample is null)
        {
            return null;
        }

        var unitSnapshot = sample.Volume.UnitId.HasValue
            ? await unitSnapshotRepository.GetByIdAsync(sample.Volume.UnitId.Value, cancellationToken)
            : null;

        return SampleDto.FromDomain(sample, unitSnapshot);
    }

    public async Task<ICollection<SampleDto>> GetAllByOrderIdAsync(
        Guid orderId,
        CancellationToken cancellationToken = default)
    {
        var samples = await sampleRepository.GetByOrderIdAsync(new OrderId(orderId), cancellationToken);
        if (samples.Count == 0)
        {
            return [];
        }

        var unitIds = samples.Where(s => s.Volume.UnitId.HasValue)
            .Select(s => s.Volume.UnitId!.Value)
            .Distinct()
            .ToList();

        var unitsDictionary = unitIds.Count == 0
            ? new Dictionary<UnitId, UnitSnapshot>()
            : (await unitSnapshotRepository.GetByIdsAsync(unitIds, cancellationToken)).ToDictionary(u => u.Id, u => u);

        return samples.Select(s =>
            {
                var unit = s.Volume.UnitId.HasValue && unitsDictionary.TryGetValue(s.Volume.UnitId.Value, out var u)
                    ? u
                    : null;
                return SampleDto.FromDomain(s, unit);
            })
            .ToList();
    }
}
