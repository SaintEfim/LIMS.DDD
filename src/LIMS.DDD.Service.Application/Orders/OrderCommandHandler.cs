using LIMS.DDD.Service.Application.Orders.Commands;
using LIMS.DDD.Service.Domain.LaboratoryOperationsContext.OrderAggregate;
using LIMS.DDD.Service.Domain.LaboratoryOperationsContext.OrderAggregate.ValueObjects;
using LIMS.DDD.Service.Domain.LaboratoryOperationsContext.SampleAggregate;
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
        if (nameResult.IsFailure) return Result<Order, Exception>.Failure(nameResult.Error!);

        var descResult = Description.Create(command.Description);
        if (descResult.IsFailure) return Result<Order, Exception>.Failure(descResult.Error!);

        var codeResult = Code.Create(command.Code);
        if (codeResult.IsFailure) return Result<Order, Exception>.Failure(codeResult.Error!);

        var createResult = Order.Create(nameResult.GetValue(), descResult.GetValue(), command.Contractor,
            codeResult.GetValue());
        if (createResult.IsFailure) return Result<Order, Exception>.Failure(createResult.Error!);

        try
        {
            repository.Add(createResult.GetValue());
            await repository.SaveChangesAsync(cancellationToken);
            return Result<Order, Exception>.Success(createResult.GetValue());
        }
        catch (Exception ex)
        {
            return Result<Order, Exception>.Failure(new Exception("Failed to save Order.", ex));
        }
    }

    public async Task<Result<Exception>> UpdateAsync(
        Guid id,
        UpdateOrderCommand command,
        CancellationToken cancellationToken = default)
    {
        var orderResult = await GetOrderForChangeAsync(id, cancellationToken);
        if (orderResult.IsFailure) return Result<Exception>.Failure(orderResult.Error!);

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
        if (updateResult.IsFailure) return Result<Exception>.Failure(updateResult.Error!);

        await repository.SaveChangesAsync(cancellationToken);
        return Result<Exception>.Success();
    }

    public async Task<Result<Exception>> ChangeStatusAsync(
        Guid id,
        ChangeOrderStatusCommand command,
        CancellationToken cancellationToken = default)
    {
        var orderResult = await GetOrderForChangeAsync(id, cancellationToken);
        if (orderResult.IsFailure) return Result<Exception>.Failure(orderResult.Error!);

        if (!OrderStatus.TryParse(command.Status, out var newStatus))
            return Result<Exception>.Failure(new InvalidOperationException($"Unknown status '{command.Status}'."));

        var changeResult = orderResult.GetValue()
            .ChangeStatus(newStatus);
        if (changeResult.IsFailure) return Result<Exception>.Failure(changeResult.Error!);

        await repository.SaveChangesAsync(cancellationToken);
        return Result<Exception>.Success();
    }

    public async Task<Result<Exception>> DeleteAsync(
        Guid id,
        CancellationToken cancellationToken = default)
    {
        var orderResult = await GetOrderForChangeAsync(id, cancellationToken);
        if (orderResult.IsFailure) return Result<Exception>.Failure(orderResult.Error!);

        var order = orderResult.GetValue();

        var deleteResult = order.Delete();
        if (deleteResult.IsFailure) return Result<Exception>.Failure(deleteResult.Error!);

        await repository.SaveChangesAsync(cancellationToken);

        return Result<Exception>.Success();
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
