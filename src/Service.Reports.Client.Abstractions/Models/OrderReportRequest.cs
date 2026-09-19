namespace Service.Reports.Client.Models;

public sealed record OrderReportRequest(OrderInfo? Order, IReadOnlyList<SampleInfo>? Samples);

public sealed record OrderInfo(
    Guid Id,
    string Name,
    string? Description,
    string? Code,
    string Contractor,
    string Status);

public sealed record SampleInfo(
    Guid Id,
    Guid OrderId,
    string Name,
    DateTimeOffset? GatherDateBegin,
    DateTimeOffset? GatherDateEnd,
    string? Code,
    double? VolumeValue,
    string? VolumeUnit,
    string Status);
