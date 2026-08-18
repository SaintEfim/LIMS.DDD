using LIMS.Service.LaboratoryOperations.Domain.SampleAggregate;

namespace LIMS.Service.LaboratoryOperations.Application.Samples;

public sealed record SampleDto(
    Guid Id,
    Guid OrderId,
    string Name,
    DateTimeOffset? GatherDateBegin,
    DateTimeOffset? GatherDateEnd,
    string? Code,
    double? VolumeValue,
    string? VolumeUnit,
    string Status)
{
    public static SampleDto FromDomain(
        Sample sample)
    {
        return new SampleDto(sample.Id.Value, sample.OrderId.Value, sample.Name.Value, sample.GatherDate.Begin,
            sample.GatherDate.End, sample.Code.Value, sample.Volume.Value, sample.Volume.Unit,
            sample.SampleStatus.Name);
    }
}
