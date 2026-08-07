namespace LIMS.DDD.Service.Application.Samples.Commands;

public sealed record CreateSampleCommand(
    string Name,
    DateTimeOffset? GatherDateBegin,
    DateTimeOffset? GatherDateEnd,
    string Code,
    double? VolumeValue,
    string? VolumeUnit);
