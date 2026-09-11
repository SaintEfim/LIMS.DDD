using Guides.Service.Domain;
using Library.Application.SeedWork;
using Library.Application.SeedWork.Errors;
using Library.Domain.SeedWork;
using Library.Domain.SeedWork.Result;
using Library.Domain.SeedWork.ValueObjects;

namespace Guides.Service.Application.Commands;

public sealed class UnitCommandsHandler(
    IUnitOfWork unitOfWork,
    IUnitRepository repository) : ICommandsHandler
{
    public async Task<Result<Unit, ApplicationError>> CreateAsync(
        CreateUnitCommand message,
        CancellationToken cancellationToken = default)
    {
        var nameResult = Name.Create(message.Name);
        if (nameResult.IsFailure)
        {
            return new DomainRuleViolation(nameResult.GetError());
        }

        var t = new Unit(nameResult.GetValue());

        return await SaveNewAsync(t, cancellationToken);
    }

    private async Task<Result<Unit, ApplicationError>> SaveNewAsync(
        Unit unit,
        CancellationToken cancellationToken = default)
    {
        try
        {
            repository.Add(unit);
            await unitOfWork.SaveChangesAsync(cancellationToken);

            return unit;
        }
        catch (Exception ex)
        {
            return new PersistenceError($"Failed to save Unitt: {ex.Message}");
        }
    }
}
