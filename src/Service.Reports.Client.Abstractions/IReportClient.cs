using Service.Reports.Client.Models;

namespace Service.Reports.Client;

public interface IReportClient
{
    Task<byte[]> GenerateReportAsync(
        OrderInfo order,
        IReadOnlyList<SampleInfo> samples,
        CancellationToken cancellationToken = default);
}
