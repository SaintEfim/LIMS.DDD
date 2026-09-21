using LIMS.Service.LaboratoryOperations.Application.BackgroundOperations;
using LIMS.Service.LaboratoryOperations.Application.Orders;
using LIMS.Service.LaboratoryOperations.Application.Samples;
using Microsoft.Extensions.Logging;
using Service.Reports.Client;
using Service.Reports.Client.Models;

namespace LIMS.Service.LaboratoryOperations.Infrastructure.BackgroundOperations;

internal sealed class ReportProcessor(
    IReportClient reportClient,
    BackgroundOperationCommandsHandler commands,
    OrderQueries orderQueries,
    SampleQueries sampleQueries,
    ILogger<ReportProcessor> logger) : BackgroundOperationProcessor<ReportPayload>(commands, logger)
{
    protected override async Task<string> ProcessAsync(
        ReportPayload payload,
        CancellationToken cancellationToken)
    {
        if (payload.OrderId == Guid.Empty) throw new ArgumentException("Report payload must contain a valid order id.");

        var order = await orderQueries.GetByIdAsync(payload.OrderId, cancellationToken) ??
                    throw new InvalidOperationException($"Order with id '{payload.OrderId}' was not found.");
        var samples = await sampleQueries.GetAllByOrderIdAsync(payload.OrderId, cancellationToken);
        var reportOrder = new OrderInfo(order.Id, order.Name, order.Description, order.Code, order.Contractor,
            order.Status);
        var reportSamples = samples.Select(sample => new SampleInfo(sample.Id, sample.OrderId, sample.Name,
                sample.GatherDateBegin, sample.GatherDateEnd, sample.Code, sample.VolumeValue, sample.VolumeUnit,
                sample.Status))
            .ToList();

        var pdf = await reportClient.GenerateReportAsync(reportOrder, reportSamples, cancellationToken);
        return Convert.ToBase64String(pdf);
    }
}
