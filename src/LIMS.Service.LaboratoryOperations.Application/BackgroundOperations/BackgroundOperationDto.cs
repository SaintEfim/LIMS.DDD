using LIMS.Service.LaboratoryOperations.Domain.BackgroundOperations;
using Newtonsoft.Json.Linq;

namespace LIMS.Service.LaboratoryOperations.Application.BackgroundOperations;

public sealed record BackgroundOperationDto(
    Guid Id,
    Guid RequestedByUserId,
    OperationType Type,
    OperationStatus Status,
    JObject Payload,
    string? Result,
    string? Error,
    DateTime CreatedAtUtc,
    DateTime? StartedAtUtc,
    DateTime? CompletedAtUtc)
{
    public static BackgroundOperationDto FromDomain(
        BackgroundOperation operation) =>
        new(operation.Id, operation.RequestedByUserId, operation.Type, operation.Status, operation.Payload,
            operation.Result, operation.Error, operation.CreatedAtUtc, operation.StartedAtUtc,
            operation.CompletedAtUtc);
}
