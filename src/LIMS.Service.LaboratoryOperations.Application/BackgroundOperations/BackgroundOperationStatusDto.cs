using System.Text.Json.Serialization;
using LIMS.Service.LaboratoryOperations.Domain.BackgroundOperations;

namespace LIMS.Service.LaboratoryOperations.Application.BackgroundOperations;

public sealed record BackgroundOperationStatusDto(
    Guid OperationId,
    [property: JsonConverter(typeof(JsonStringEnumConverter<OperationType>))] OperationType Type,
    [property: JsonConverter(typeof(JsonStringEnumConverter<OperationStatus>))] OperationStatus Status,
    string? Error)
{
    public static BackgroundOperationStatusDto FromOperation(BackgroundOperationDto operation) =>
        new(operation.Id, operation.Type, operation.Status, operation.Error);
}
