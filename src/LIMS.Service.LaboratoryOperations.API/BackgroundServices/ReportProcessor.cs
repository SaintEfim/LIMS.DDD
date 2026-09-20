using LIMS.Service.LaboratoryOperations.API.Hubs;
using LIMS.Service.LaboratoryOperations.Application.BackgroundOperations;
using LIMS.Service.LaboratoryOperations.Application.Orders;
using LIMS.Service.LaboratoryOperations.Application.Samples;
using Microsoft.AspNetCore.SignalR;
using Service.Reports.Client;
using Service.Reports.Client.Models;

namespace LIMS.Service.LaboratoryOperations.API.BackgroundServices;

public sealed class ReportProcessor(
    IHubContext<NotificationHub> hubContext,
    IReportClient reportClient,
    BackgroundOperationServices services,
    OrderQueries orderQueries,
    SampleQueries sampleQueries,
    ILogger<ReportProcessor> logger) : IProcessor
{
    public sealed record ReportPayload(Guid OrderId);

    public async Task ExecuteAsync(
        BackgroundOperationDto operation,
        CancellationToken cancellationToken = default)
    {
        var startResult = await services.Commands.StartAsync(operation.Id, cancellationToken);
        if (startResult.IsFailure)
        {
            logger.LogWarning("Background operation {OperationId} was not started: {Error}.", operation.Id, startResult
                .GetError()
                .Message);
            return;
        }

        try
        {
            var payload = operation.Payload.ToObject<ReportPayload>();
            if (payload is null || payload.OrderId == Guid.Empty)
            {
                await FailAsync(operation.Id, "Report payload must contain a valid order id.", cancellationToken);
                return;
            }

            var order = await orderQueries.GetByIdAsync(payload.OrderId, cancellationToken);
            if (order is null)
            {
                await FailAsync(operation.Id, $"Order with id '{payload.OrderId}' was not found.", cancellationToken);
                return;
            }

            var samples = await sampleQueries.GetAllByOrderIdAsync(payload.OrderId, cancellationToken);
            var reportOrder = new OrderInfo(order.Id, order.Name, order.Description, order.Code, order.Contractor,
                order.Status);
            var reportSamples = samples.Select(sample => new SampleInfo(sample.Id, sample.OrderId, sample.Name,
                    sample.GatherDateBegin, sample.GatherDateEnd, sample.Code, sample.VolumeValue, sample.VolumeUnit,
                    sample.Status))
                .ToList();

            var pdf = await reportClient.GenerateReportAsync(reportOrder, reportSamples, cancellationToken);
            var completionResult = await services.Commands.SucceedAsync(
                operation.Id, Convert.ToBase64String(pdf), cancellationToken);

            if (completionResult.IsFailure)
            {
                logger.LogError("Background operation {OperationId} generated a report but was not completed: {Error}.",
                    operation.Id, completionResult.GetError()
                        .Message);
            }
        }
        catch (Exception exception) when (exception is not OperationCanceledException ||
                                          !cancellationToken.IsCancellationRequested)
        {
            logger.LogError(exception, "Report generation failed for background operation {OperationId}.",
                operation.Id);
            await FailAsync(operation.Id, exception.Message, cancellationToken);
        }
    }

    private async Task FailAsync(
        Guid operationId,
        string error,
        CancellationToken cancellationToken)
    {
        var failResult = await services.Commands.FailAsync(operationId, error, cancellationToken);
        if (failResult.IsFailure)
        {
            logger.LogError("Background operation {OperationId} was not marked as failed: {Error}.", operationId,
                failResult.GetError()
                    .Message);
        }
    }
}
