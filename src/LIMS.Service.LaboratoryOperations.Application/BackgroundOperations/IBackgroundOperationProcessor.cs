namespace LIMS.Service.LaboratoryOperations.Application.BackgroundOperations;

public interface IBackgroundOperationProcessor
{
    Task ExecuteAsync(
        BackgroundOperationDto operation,
        CancellationToken cancellationToken = default);
}
