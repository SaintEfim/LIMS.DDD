using LIMS.Service.LaboratoryOperations.Application.BackgroundOperations;

namespace LIMS.Service.LaboratoryOperations.API.BackgroundServices;

public interface IProcessor
{
    Task ExecuteAsync(
        BackgroundOperationDto operation,
        CancellationToken cancellationToken = default);
}
