using Application.SeedWork;
using Application.SeedWork.Errors;
using Domain.SeedWork;
using Domain.SeedWork.Result;
using Domain.SeedWork.ValueObjects;
using LIMS.Service.LaboratoryOperations.Domain.UnitSnapshots;

namespace LIMS.Service.LaboratoryOperations.Application.Units;

public sealed class UnitSnapshotCommandsHandler(
    IUnitOfWork unitOfWork,
    IUnitSnapshotRepository snapshotRepository) : ICommandsHandler
{
    public async Task<Result<UnitSnapshot, ApplicationError>> CreateAsync(
        CreateUnitSnapshotCommand message,
        CancellationToken cancellationToken = default)
    {
        var unitId = new UnitId(message.Id);

        var nameResult = Name.Create(message.Name);
        if (nameResult.IsFailure)
        {
            return new DomainRuleViolation(nameResult.GetError());
        }

        var snapshot = new UnitSnapshot(unitId, nameResult.GetValue());

        return await SaveNewAsync(snapshot, cancellationToken);
    }

    private async Task<Result<UnitSnapshot, ApplicationError>> SaveNewAsync(
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
            return new PersistenceError($"Failed to save UnitSnapshot: {ex.Message}");
        }
    }
}
