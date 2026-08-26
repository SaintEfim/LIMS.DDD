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

    public string EntityName { get; }
    public string FromStatus { get; }
    public string ToStatus { get; }

    public override string Code => "INVALID_STATUS_TRANSITION";
}

public sealed class EntityNotEditableError(string entityName, string statusName, string operation)
    : DomainError($"Cannot {operation} while {entityName} is in '{statusName}' status.")
{
    public override string Code => "ENTITY_NOT_EDITABLE";
}

public sealed class EntityInUseError(string entityName, string usedBy)
    : DomainError($"{entityName} cannot be removed because it is used by {usedBy}.")
{
    public override string Code => "ENTITY_IN_USE";
}

public sealed class DuplicateEntityError(string entityName, string fieldName, object fieldValue)
    : DomainError($"{entityName} with {fieldName} '{fieldValue}' already exists.")
{
    public override string Code => "DUPLICATE_ENTITY";
}
