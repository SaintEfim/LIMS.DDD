using Domain.SeedWork.Errors;

namespace Application.SeedWork.Errors;

public abstract class ApplicationError : Exception;

public sealed class DomainRuleViolation(DomainError error) : ApplicationError
{
    public DomainError Error { get; init; } = error;

    public void Deconstruct(
        out DomainError error)
    {
        error = Error;
    }
}

public sealed class NotFoundError(string Message) : ApplicationError
{
    public void Deconstruct(
        out string message)
    {
        message = this.Message;
    }
}

public sealed class ValidationError(string Message) : ApplicationError
{
    public void Deconstruct(
        out string message)
    {
        message = this.Message;
    }
}

public sealed class PersistenceError(string Message) : ApplicationError
{
    public void Deconstruct(
        out string message)
    {
        message = this.Message;
    }
}
