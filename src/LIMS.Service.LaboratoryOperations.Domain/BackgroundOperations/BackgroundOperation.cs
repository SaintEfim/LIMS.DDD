using Library.Domain.SeedWork;
using Library.Domain.SeedWork.Errors;
using Library.Domain.SeedWork.Result;
using Newtonsoft.Json.Linq;

namespace LIMS.Service.LaboratoryOperations.Domain.BackgroundOperations;

public sealed class BackgroundOperation : IAggregateRoot
{
    private BackgroundOperation()
    {
    }

    public BackgroundOperation(
        Guid requestedByUserId,
        OperationType type,
        JObject payload)
    {
        if (requestedByUserId == Guid.Empty)
        {
            throw new ArgumentException("Requested-by user id must be specified.", nameof(requestedByUserId));
        }

        if (payload is null)
        {
            throw new ArgumentException("Operation payload must be specified.", nameof(payload));
        }

        Id = Guid.NewGuid();
        RequestedByUserId = requestedByUserId;
        Type = type;
        Payload = payload;
        CreatedAtUtc = DateTime.UtcNow;
        Status = OperationStatus.Pending;
    }

    public Guid Id { get; init; }
    public Guid RequestedByUserId { get; init; }
    public OperationType Type { get; init; }
    public OperationStatus Status { get; private set; }
    public JObject Payload { get; init; } = null!;
    public string? Result { get; private set; }
    public string? Error { get; private set; }
    public DateTime CreatedAtUtc { get; init; }
    public DateTime? StartedAtUtc { get; private set; }
    public DateTime? CompletedAtUtc { get; private set; }

    public Result<None, InvalidStatusTransitionError> Start()
    {
        if (Status != OperationStatus.Pending)
        {
            return InvalidTransition(OperationStatus.Running);
        }

        Status = OperationStatus.Running;
        StartedAtUtc = DateTime.UtcNow;
        return new None();
    }

    public Result<None, InvalidStatusTransitionError> Succeed(
        string result)
    {
        if (Status != OperationStatus.Running)
        {
            return InvalidTransition(OperationStatus.Succeeded);
        }

        Status = OperationStatus.Succeeded;
        Result = result;
        Error = null;
        CompletedAtUtc = DateTime.UtcNow;
        return new None();
    }

    public Result<None, InvalidStatusTransitionError> Fail(
        string error)
    {
        if (Status != OperationStatus.Running)
        {
            return InvalidTransition(OperationStatus.Failed);
        }

        if (string.IsNullOrWhiteSpace(error))
        {
            throw new ArgumentException("Operation error must be specified.", nameof(error));
        }

        Status = OperationStatus.Failed;
        Error = error;
        CompletedAtUtc = DateTime.UtcNow;
        return new None();
    }

    public Result<None, InvalidStatusTransitionError> Cancel()
    {
        if (Status is not (OperationStatus.Pending or OperationStatus.Running))
        {
            return InvalidTransition(OperationStatus.Canceled);
        }

        Status = OperationStatus.Canceled;
        CompletedAtUtc = DateTime.UtcNow;
        return new None();
    }

    private InvalidStatusTransitionError InvalidTransition(
        OperationStatus targetStatus)
    {
        return new InvalidStatusTransitionError(nameof(BackgroundOperation), Status.ToString(),
            targetStatus.ToString());
    }
}
