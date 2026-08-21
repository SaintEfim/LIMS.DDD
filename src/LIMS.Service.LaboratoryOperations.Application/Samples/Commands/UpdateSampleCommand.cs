namespace LIMS.Service.LaboratoryOperations.Application.Samples.Commands;

public sealed record UpdateSampleCommand(
    string? Name,
    DateTimeOffset? GatherDateBegin,
    DateTimeOffset? GatherDateEnd,
    string? Code,
    double? VolumeValue,
    Guid? VolumeUnitId);
