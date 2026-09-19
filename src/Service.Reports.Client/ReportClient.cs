using System.Net.Http.Json;
using Service.Reports.Client.Models;

namespace Service.Reports.Client;

public class ReportClient(IHttpClientFactory httpClientFactory) : IReportClient
{
    public async Task<byte[]> GenerateReportAsync(
        OrderInfo order,
        IReadOnlyList<SampleInfo> samples,
        CancellationToken cancellationToken = default)
    {
        var client = httpClientFactory.CreateClient("Reports");
        using var response = await client.PostAsJsonAsync("/reports", new
        {
            order,
            samples
        }, cancellationToken);

        if (!response.IsSuccessStatusCode)
            throw new HttpRequestException("Report service failed to generate the report.", inner: null,
                statusCode: response.StatusCode);

        return await response.Content.ReadAsByteArrayAsync(cancellationToken);
    }
}
