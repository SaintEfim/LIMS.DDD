using Domain.SeedWork.SeedWork;
using Domain.SeedWork.SeedWork.Result;
using Domain.SeedWork.SeedWork.ValueObjects;
using LIMS.Service.LaboratoryOperations.Domain.UnitSnapshots;

namespace LIMS.Service.LaboratoryOperations.Application.Units;

public sealed class UnitSnapshotCommandsHandler(IUnitOfWork unitOfWork, IUnitSnapshotRepository snapshotRepository)
{
    public async Task<Result<UnitSnapshot, Exception>> CreateAsync(
        CreateUnitSnapshotCommand message,
        CancellationToken cancellationToken = default)
    {
        var idResult = new UnitId(message.Id);

        var nameResult = Name.Create(message.Name);
        if (nameResult.IsFailure)
        {
            return nameResult.CastFailure<UnitSnapshot>();
        }

        var snapshot = new UnitSnapshot(idResult, nameResult.GetValue());

        return await SaveNewAsync(snapshot, cancellationToken);
    }

    private async Task<Result<UnitSnapshot, Exception>> SaveNewAsync(
        UnitSnapshot snapshot,
        CancellationToken cancellationToken = default)
    {
        try
        {
            snapshotRepository.Add(snapshot);
            await unitOfWork.SaveChangesAsync(cancellationToken);

            return Result<UnitSnapshot, Exception>.Success(snapshot);
        }
        catch (Exception ex)
        {
            return Result<UnitSnapshot, Exception>.Failure(new Exception($"Failed to save UnitSnapshot: {ex.Message}",
                ex));
        }
    }
}
