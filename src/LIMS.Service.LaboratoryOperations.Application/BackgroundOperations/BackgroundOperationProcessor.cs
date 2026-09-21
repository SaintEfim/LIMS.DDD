using Microsoft.Extensions.Logging;

namespace LIMS.Service.LaboratoryOperations.Application.BackgroundOperations;

public abstract class BackgroundOperationProcessor<TPayload>(
    BackgroundOperationCommandsHandler commands,
    ILogger logger) : IBackgroundOperationProcessor
    where TPayload : class
{
    public async Task ExecuteAsync(
        BackgroundOperationDto operation,
        CancellationToken cancellationToken = default)
    {
        var start = await commands.StartAsync(operation.Id, cancellationToken);
        if (start.IsFailure)
        {
            logger.LogWarning("Background operation {OperationId} was not started: {Error}.", operation.Id, start
                .GetError()
                .Message);
            return;
        }

        string result;
        try
        {
            var payload = operation.Payload.ToObject<TPayload>() ??
                          throw new InvalidOperationException("Operation payload is missing.");
            result = await ProcessAsync(payload, cancellationToken);
        }
        catch (OperationCanceledException) when (cancellationToken.IsCancellationRequested)
        {
            throw;
        }
        catch (Exception exception)
        {
            logger.LogError(exception, "Background operation {OperationId} failed.", operation.Id);
            var failure = await commands.FailAsync(operation.Id, exception.Message, cancellationToken);
            if (failure.IsFailure)
            {
                logger.LogError("Background operation {OperationId} was not marked as failed: {Error}.", operation.Id,
                    failure.GetError()
                        .Message);
            }

            return;
        }

        var completion = await commands.SucceedAsync(operation.Id, result, cancellationToken);
        if (completion.IsFailure)
        {
            logger.LogError("Background operation {OperationId} result was not saved: {Error}.", operation.Id,
                completion.GetError()
                    .Message);
        }
    }

    protected abstract Task<string> ProcessAsync(
        TPayload payload,
        CancellationToken cancellationToken);
}
