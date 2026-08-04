using LIMS.DDD.Service.Domain.LaboratoryOperationsContext.SampleAggregate;

namespace LIMS.DDD.Service.Application.LaboratoryOperations.Samples.Queries;

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
    public static SampleDto FromDomain(Sample sample)
    {
        return new SampleDto(
            Id: sample.Id.Value,
            OrderId: sample.OrderId.Value,
            Name: sample.Name.Value,
            GatherDateBegin: sample.GatherDate.Begin,
            GatherDateEnd: sample.GatherDate.End,
            Code: sample.Code?.Value,
            VolumeValue: sample.Volume.Value,
            VolumeUnit: sample.Volume.Unit,
            Status: sample.SampleStatus.Name);
    }
}
