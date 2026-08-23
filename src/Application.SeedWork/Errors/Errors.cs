using Domain.SeedWork.Errors;

namespace Application.SeedWork.Errors;

public abstract class ApplicationError : Exception;

public sealed class DomainRuleViolation(DomainError error) : ApplicationError
{
    public DomainError Error { get; init; } = error;
}

public sealed class NotFoundError(string Message) : ApplicationError;

public sealed class ValidationError(string Message) : ApplicationError;

public sealed class PersistenceError(string Message) : ApplicationError;
