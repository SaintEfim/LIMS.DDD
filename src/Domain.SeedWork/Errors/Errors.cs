namespace Domain.SeedWork.Errors;

public abstract class DomainException : Exception
{
    protected DomainException(
        string message)
        : base(message)
    {
    }

    protected DomainException(
        string message,
        Exception? innerException)
        : base(message, innerException)
    {
    }

    public abstract string Code { get; }
}

public sealed class EntityNotFoundException(string entityName, object entityId)
    : DomainException($"{entityName} with id '{entityId}' was not found.")
{
    public string EntityName { get; } = entityName;
    public object EntityId { get; } = entityId;
    public override string Code => "ENTITY_NOT_FOUND";
}

public sealed class EntityAlreadyDeletedException(string entityName, object entityId)
    : DomainException($"{entityName} with id '{entityId}' is already deleted.")
{
    public override string Code => "ENTITY_ALREADY_DELETED";
}

public sealed class ValidationException(string message) : DomainException(message)
{
    public override string Code => "VALIDATION_ERROR";
}

public sealed class PersistenceException(string message, Exception inner) : DomainException(message, inner)
{
    public override string Code => "PERSISTENCE_ERROR";
}
