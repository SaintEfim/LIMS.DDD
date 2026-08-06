using LIMS.DDD.Service.Application.Orders.Commands;
using LIMS.DDD.Service.Domain.LaboratoryOperationsContext.OrderAggregate;
using LIMS.DDD.Service.Domain.LaboratoryOperationsContext.OrderAggregate.ValueObjects;
using LIMS.DDD.Service.Domain.LaboratoryOperationsContext.ValueObjects;
using LIMS.DDD.Service.Domain.SeedWork.Result;
using LIMS.DDD.Service.Domain.SeedWork.ValueObjects;

namespace LIMS.DDD.Service.Application.Orders;

public sealed class OrderCommandHandler(IOrderRepository repository)
{
    public async Task<Result<Order, Exception>> CreateAsync(
        CreateOrderCommand command,
        CancellationToken cancellationToken = default)
    {
        var nameResult = Name.Create(command.Name);
        if (nameResult.IsFailure)
        {
            return nameResult.CastFailure<Order>();
        }

        var descResult = Description.Create(command.Description);
        if (descResult.IsFailure)
        {
            return descResult.CastFailure<Order>();
        }

        var codeResult = Code.Create(command.Code);
        if (codeResult.IsFailure)
        {
            return codeResult.CastFailure<Order>();
        }

        var createResult = Order.Create(nameResult.GetValue(), descResult.GetValue(), command.Contractor,
            codeResult.GetValue());
        if (createResult.IsFailure)
        {
            return createResult.CastFailure<Order>();
        }

        return await SaveNewAsync(createResult.GetValue(), cancellationToken);
    }

    public async Task<Result<None, Exception>> UpdateAsync(
        Guid id,
        UpdateOrderCommand command,
        CancellationToken cancellationToken = default)
    {
        var orderResult = await GetOrderForChangeAsync(id, cancellationToken);
        if (orderResult.IsFailure)
        {
            return orderResult.CastFailure<None>();
        }

        var order = orderResult.GetValue();

        var name = command.Name is not null
            ? Name.Create(command.Name)
                .GetValue()
            : null;
        var desc = command.Description is not null
            ? Description.Create(command.Description)
                .GetValue()
            : null;
        var code = command.Code is not null
            ? Code.Create(command.Code)
                .GetValue()
            : null;

        var updateResult = order.UpdatePartial(name, desc, command.Contractor, code);
        if (updateResult.IsFailure)
        {
            return updateResult.CastFailure<None>();
        }

        await repository.SaveChangesAsync(cancellationToken);
        return Result<None, Exception>.Success();
    }

    public async Task<Result<None, Exception>> ChangeStatusAsync(
        Guid id,
        ChangeOrderStatusCommand command,
        CancellationToken cancellationToken = default)
    {
        var orderResult = await GetOrderForChangeAsync(id, cancellationToken);
        if (orderResult.IsFailure)
        {
            return orderResult.CastFailure<None>();
        }

        if (!OrderStatus.TryParse(command.Status, out var newStatus))
        {
            return Result<None, Exception>.Failure(
                new InvalidOperationException($"Unknown status '{command.Status}'."));
        }

        var changeResult = orderResult.GetValue()
            .ChangeStatus(newStatus);
        if (changeResult.IsFailure)
        {
            return changeResult.CastFailure<None>();
        }

        await repository.SaveChangesAsync(cancellationToken);
        return Result<None, Exception>.Success();
    }

    public async Task<Result<None, Exception>> DeleteAsync(
        Guid id,
        CancellationToken cancellationToken = default)
    {
        var orderResult = await GetOrderForChangeAsync(id, cancellationToken);
        if (orderResult.IsFailure)
        {
            return orderResult.CastFailure<None>();
        }

        var order = orderResult.GetValue();

        var deleteResult = order.Delete();
        if (deleteResult.IsFailure)
        {
            return deleteResult.CastFailure<None>();
        }

        await repository.SaveChangesAsync(cancellationToken);

        return Result<None, Exception>.Success();
    }

    private async Task<Result<Order, Exception>> SaveNewAsync(
        Order order,
        CancellationToken cancellationToken)
    {
        try
        {
            repository.Add(order);
            await repository.SaveChangesAsync(cancellationToken);
            return Result<Order, Exception>.Success(order);
        }
        catch (Exception ex)
        {
            return Result<Order, Exception>.Failure(new Exception($"Failed to save Order: {ex.Message}", ex));
        }
    }

    private async Task<Result<Order, Exception>> GetOrderForChangeAsync(
        Guid id,
        CancellationToken ct)
    {
        var order = await repository.GetByIdForChangeAsync(new OrderId(id), ct);
        return order is null
            ? Result<Order, Exception>.Failure(new KeyNotFoundException($"Order with id {id} not found."))
            : Result<Order, Exception>.Success(order);
    }
}
