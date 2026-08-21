using Application.SeedWork;
using Domain.SeedWork;
using Domain.SeedWork.Errors;
using Domain.SeedWork.Result;
using Domain.SeedWork.ValueObjects;
using LIMS.Service.Methodologies.Domain.UnitSnapshots;

namespace LIMS.Service.Methodologies.Application.Units;

public sealed class UnitSnapshotCommandsHandler(
    IUnitOfWork unitOfWork,
    IUnitSnapshotRepository snapshotRepository) : ICommandsHandler
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

            return snapshot;
        }
        catch (Exception ex)
        {
            return new PersistenceException($"Failed to save UnitSnapshot: {ex.Message}", ex);
        }
    }
}
