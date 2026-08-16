using LIMS.DDD.Service.Domain.SeedWork;
using LIMS.DDD.Service.Domain.SeedWork.Result;
using LIMS.DDD.Service.Domain.SeedWork.Snapshots;
using LIMS.DDD.Service.Domain.SeedWork.ValueObjects;

namespace LIMS.DDD.Service.Application.Units;

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

        var snapshotResult = UnitSnapshot.Create(idResult, nameResult.GetValue());
        if (snapshotResult.IsFailure)
        {
            return snapshotResult.CastFailure<UnitSnapshot>();
        }

        return await SaveNewAsync(snapshotResult.GetValue(), cancellationToken);
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
