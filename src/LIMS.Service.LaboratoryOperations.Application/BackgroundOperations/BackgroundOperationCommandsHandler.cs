using Library.Application.SeedWork;
using Library.Application.SeedWork.Errors;
using Library.Domain.SeedWork;
using Library.Domain.SeedWork.Errors;
using Library.Domain.SeedWork.Result;
using LIMS.Service.LaboratoryOperations.Domain.BackgroundOperations;
using ValidationError = Library.Application.SeedWork.Errors.ValidationError;

namespace LIMS.Service.LaboratoryOperations.Application.BackgroundOperations;

public sealed class BackgroundOperationCommandsHandler(
    IUnitOfWork unitOfWork,
    IBackgroundOperationRepository repository) : ICommandsHandler
{
    public async Task<Result<BackgroundOperation, ApplicationError>> CreateAsync(
        CreateBackgroundOperationCommand command,
        CancellationToken cancellationToken = default)
    {
        try
        {
            var operation = new BackgroundOperation(command.RequestedByUserId, command.Type, command.Payload);
            repository.Add(operation);
            await unitOfWork.SaveChangesAsync(cancellationToken);
            return operation;
        }
        catch (ArgumentException exception)
        {
            return new ValidationError(exception.Message);
        }
        catch (Exception exception) when (exception is not OperationCanceledException || !cancellationToken.IsCancellationRequested)
        {
            return new PersistenceError($"Failed to create background operation: {exception.Message}");
        }
    }

    public Task<Result<None, ApplicationError>> StartAsync(
        Guid id,
        CancellationToken cancellationToken = default) =>
        ChangeAsync(id, operation => operation.Start(), cancellationToken);

    public Task<Result<None, ApplicationError>> SucceedAsync(
        Guid id,
        string result,
        CancellationToken cancellationToken = default) =>
        ChangeAsync(id, operation => operation.Succeed(result), cancellationToken);

    public Task<Result<None, ApplicationError>> FailAsync(
        Guid id,
        string error,
        CancellationToken cancellationToken = default) =>
        ChangeAsync(id, operation => operation.Fail(error), cancellationToken);

    public Task<Result<None, ApplicationError>> CancelAsync(
        Guid id,
        CancellationToken cancellationToken = default) =>
        ChangeAsync(id, operation => operation.Cancel(), cancellationToken);

    private async Task<Result<None, ApplicationError>> ChangeAsync(
        Guid id,
        Func<BackgroundOperation, Result<None, InvalidStatusTransitionError>> change,
        CancellationToken cancellationToken)
    {
        var operation = await repository.GetByIdForChangeAsync(id, cancellationToken);
        if (operation is null)
        {
            return new NotFoundError($"Background operation with id '{id}' not found.");
        }

        try
        {
            var changeResult = change(operation);
            if (changeResult.IsFailure)
            {
                return new DomainRuleViolation(changeResult.GetError());
            }

            await unitOfWork.SaveChangesAsync(cancellationToken);
            return new None();
        }
        catch (ArgumentException exception)
        {
            return new ValidationError(exception.Message);
        }
        catch (Exception exception) when (exception is not OperationCanceledException || !cancellationToken.IsCancellationRequested)
        {
            return new PersistenceError($"Failed to update background operation: {exception.Message}");
        }
    }
}
