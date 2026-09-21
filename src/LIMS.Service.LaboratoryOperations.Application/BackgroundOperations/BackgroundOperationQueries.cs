using Library.Application.SeedWork;
using LIMS.Service.LaboratoryOperations.Domain.BackgroundOperations;

namespace LIMS.Service.LaboratoryOperations.Application.BackgroundOperations;

public sealed class BackgroundOperationQueries(IBackgroundOperationRepository repository) : IQueries
{
    public async Task<BackgroundOperationDto?> GetByIdAsync(
        Guid id,
        CancellationToken cancellationToken = default)
    {
        var operation = await repository.GetByIdAsync(id, cancellationToken);
        return operation is null ? null : BackgroundOperationDto.FromDomain(operation);
    }

    public async Task<IReadOnlyList<BackgroundOperationDto>> GetPendingAsync(
        int take,
        CancellationToken cancellationToken = default)
    {
        var operations = await repository.GetPendingAsync(take, cancellationToken);
        return operations.Select(BackgroundOperationDto.FromDomain).ToList();
    }
}
