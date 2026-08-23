namespace Domain.SeedWork.Errors;

public abstract class DomainError : Exception
{
    protected DomainError(
        string message)
        : base(message)
    {
    }

    protected DomainError(
        string message,
        Exception? innerException)
        : base(message, innerException)
    {
    }

    public abstract string Code { get; }
}

public sealed class EntityNotFoundError(string entityName, object entityId)
    : DomainError($"{entityName} with id '{entityId}' was not found.")
{
    public string EntityName { get; } = entityName;
    public object EntityId { get; } = entityId;
    public override string Code => "ENTITY_NOT_FOUND";
}

public sealed class EntityAlreadyDeletedError(string entityName, object entityId)
    : DomainError($"{entityName} with id '{entityId}' is already deleted.")
{
    public override string Code => "ENTITY_ALREADY_DELETED";
}

public sealed class ValidationError(string message) : DomainError(message)
{
    public override string Code => "VALIDATION_ERROR";
}

public sealed class InvalidStatusTransitionError : DomainError
{
    public string EntityName { get; }
    public string FromStatus { get; }
    public string ToStatus { get; }

    public InvalidStatusTransitionError(
        string entityName,
        string fromStatus,
        string toStatus)
        : base($"{entityName} cannot transition from '{fromStatus}' to '{toStatus}'.")
    {
        EntityName = entityName;
        FromStatus = fromStatus;
        ToStatus = toStatus;
    }

    public InvalidStatusTransitionError(
        string entityName,
        string fromStatus,
        string toStatus,
        string message)
        : base($"{entityName} cannot transition from '{fromStatus}' to '{toStatus}'. Exception message: {message}")
    {
        EntityName = entityName;
        FromStatus = fromStatus;
        ToStatus = toStatus;
    }

    public override string Code => "INVALID_STATUS_TRANSITION";
}
